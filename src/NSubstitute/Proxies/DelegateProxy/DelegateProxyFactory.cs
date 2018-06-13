using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Proxies.CastleDynamicProxy;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateProxyFactory : IProxyFactory
    {
        private const string MethodNameInsideProxyContainer = "Invoke";
        private const string IsReadOnlyAttributeFullTypeName = "System.Runtime.CompilerServices.IsReadOnlyAttribute";
        private readonly CastleDynamicProxyFactory _castleObjectProxyFactory;
        private readonly ConcurrentDictionary<Type, Type> _delegateContainerCache = new ConcurrentDictionary<Type, Type>();
        private long _typeSuffixCounter;

        public DelegateProxyFactory(CastleDynamicProxyFactory objectProxyFactory)
        {
            _castleObjectProxyFactory = objectProxyFactory;
        }
        
        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments)
        {
            if (HasItems(additionalInterfaces))
            {
                throw new SubstituteException(
                    "Can not specify additional interfaces when substituting for a delegate. " +
                    "You must specify only a single delegate type if you need to substitute for a delegate.");
            }
            if (HasItems(constructorArguments))
            {
                throw new SubstituteException("Can not provide constructor arguments when substituting for a delegate.");
            }

            return DelegateProxy(typeToProxy, callRouter);
        }

        private static bool HasItems<T>(T[] array)
        {
            return array != null && array.Length > 0;
        }

        private object DelegateProxy(Type delegateType, ICallRouter callRouter)
        {
            var delegateContainer = _delegateContainerCache.GetOrAdd(delegateType, GenerateDelegateContainerInterface);
            var invokeMethod = delegateContainer.GetMethod(MethodNameInsideProxyContainer);

            var proxy = _castleObjectProxyFactory.GenerateProxy(callRouter, delegateContainer, Type.EmptyTypes, null);
            return invokeMethod.CreateDelegate(delegateType, proxy);
        }

        private Type GenerateDelegateContainerInterface(Type delegateType)
        {
            var delegateSignature = delegateType.GetMethod("Invoke");
            var delegateParameters = delegateSignature.GetParameters();

            var typeSuffixCounter = Interlocked.Increment(ref _typeSuffixCounter);
            var delegateTypeName = delegateType.GetTypeInfo().IsGenericType
                ? delegateType.Name.Substring(0, delegateType.Name.IndexOf("`", StringComparison.Ordinal))
                : delegateType.Name;
            var typeName = string.Format(
                "DelegateContainer_{0}_{1}",
                delegateTypeName,
                typeSuffixCounter.ToString(CultureInfo.InvariantCulture));

            return _castleObjectProxyFactory.DefineDynamicType(moduleBuilder =>
            {
                var typeBuilder = moduleBuilder.DefineType(
                    typeName,
                    TypeAttributes.Abstract | TypeAttributes.Interface | TypeAttributes.Public);

                // Notice, we don't copy the custom modifiers here.
                // That's absolutely fine, as custom modifiers are ignored when delegate is constructed.
                // See the related discussion here: https://github.com/dotnet/coreclr/issues/18401
                var methodBuilder = typeBuilder
                    .DefineMethod(
                        MethodNameInsideProxyContainer,
                        MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.Public,
                        CallingConventions.Standard,
                        delegateSignature.ReturnType,
                        delegateSignature.GetParameters().Select(p => p.ParameterType).ToArray());

                // Copy original method attributes, so "out" parameters are recognized later.
                for (var i = 0; i < delegateParameters.Length; i++)
                {
                    var parameter = delegateParameters[i];

                    // Increment position by 1 to skip the implicit "this" parameter.
                    var paramBuilder = methodBuilder.DefineParameter(i + 1, parameter.Attributes, parameter.Name);

                    // Read-only parameter ('in' keyword) is recognized by presence of the special attribute.
                    // If source parameter contained that attribute, ensure to copy it to the generated method.
                    // That helps Castle to understand that parameter is read-only and cannot be mutated.
                    DefineIsReadOnlyAttributeIfNeeded(parameter, paramBuilder, moduleBuilder);
                }

                // Preserve the original delegate type in attribute, so it can be retrieved later in code.
                methodBuilder.SetCustomAttribute(
                    new CustomAttributeBuilder(
                        typeof(ProxiedDelegateTypeAttribute).GetConstructors().Single(),
                        new object[] {delegateType}));

                return typeBuilder.CreateTypeInfo().AsType();
            });
        }

        private static void DefineIsReadOnlyAttributeIfNeeded(
            ParameterInfo sourceParameter, ParameterBuilder paramBuilder, ModuleBuilder dynamicModuleBuilder)
        {
            // Read-only parameter can be by-ref only.
            if (!sourceParameter.ParameterType.IsByRef)
            {
                return;
            }

            // Lookup for the attribute using full type name.
            // That's required because compiler can embed that type directly to the client's assembly
            // as type identity doesn't matter - only full type attribute name is checked.
            var isReadOnlyAttrType = sourceParameter.CustomAttributes
                .Select(ca => ca.AttributeType)
                .FirstOrDefault(t => t.FullName.Equals(IsReadOnlyAttributeFullTypeName, StringComparison.Ordinal));

            // Parameter doesn't contain the IsReadOnly attribute.
            if (isReadOnlyAttrType == null)
            {
                return;
            }

            // If the compiler generated attribute is used (e.g. runtime doesn't contain the attribute),
            // the generated attribute type might be internal, so we cannot referecnce it in the dynamic assembly.
            // In this case use the attribute type from the dynamic assembly.
            if (!isReadOnlyAttrType.GetTypeInfo().IsVisible)
            {
                isReadOnlyAttrType = GetIsReadOnlyAttributeInDynamicModule(dynamicModuleBuilder);
            }

            paramBuilder.SetCustomAttribute(
                new CustomAttributeBuilder(isReadOnlyAttrType.GetConstructor(Type.EmptyTypes), new object[0]));
        }

        private static Type GetIsReadOnlyAttributeInDynamicModule(ModuleBuilder moduleBuilder)
        {
            var existingType = moduleBuilder.Assembly.GetType(IsReadOnlyAttributeFullTypeName, throwOnError: false, ignoreCase: false);
            if (existingType != null)
            {
                return existingType;
            }

            return moduleBuilder
                .DefineType(
                    IsReadOnlyAttributeFullTypeName,
                    TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.NotPublic,
                    typeof(Attribute))
                .CreateTypeInfo().AsType();
        }
    }
}