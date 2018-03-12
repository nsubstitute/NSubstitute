using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateProxyFactory : IProxyFactory
    {
        private const string MethodNameInsideProxyContainer = "Invoke";
        private readonly IProxyFactory _objectProxyFactory;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Type> _delegateContainerCache = new ConcurrentDictionary<Type, Type>();
        private long _typeSuffixCounter;

        public DelegateProxyFactory(IProxyFactory objectProxyFactory)
        {
            _objectProxyFactory = objectProxyFactory;

            const string dynamicAssemblyName = "NSubsituteDelegateProxyTypes";
            _moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName(dynamicAssemblyName), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(dynamicAssemblyName);
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

        private bool HasItems<T>(T[] array)
        {
            return array != null && array.Length > 0;
        }

        private object DelegateProxy(Type delegateType, ICallRouter callRouter)
        {
            var delegateContainer = _delegateContainerCache.GetOrAdd(delegateType, GenerateDelegateContainerInterface);
            var invokeMethod = delegateContainer.GetMethod(MethodNameInsideProxyContainer);

            var proxy = _objectProxyFactory.GenerateProxy(callRouter, delegateContainer, Type.EmptyTypes, null);
            return invokeMethod.CreateDelegate(delegateType, proxy);
        }

        private Type GenerateDelegateContainerInterface(Type delegateType)
        {
            lock (_moduleBuilder)
            {
                return GenerateDelegateContainerInterfaceNoLock(delegateType);
            }
        }

        private Type GenerateDelegateContainerInterfaceNoLock(Type delegateType)
        {
            var delegateSignature = delegateType.GetMethod("Invoke");

            var typeSuffixCounter = Interlocked.Increment(ref _typeSuffixCounter);
            var typeName = "DelegateContainer_" + typeSuffixCounter.ToString(CultureInfo.InvariantCulture);

            var typeBuilder = _moduleBuilder.DefineType( typeName, TypeAttributes.Abstract | TypeAttributes.Interface | TypeAttributes.Public);
            var methodBuilder = typeBuilder
                .DefineMethod(
                    MethodNameInsideProxyContainer,
                    MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.Public,
                    delegateSignature.ReturnType,
                    delegateSignature.GetParameters().Select(p => p.ParameterType).ToArray());

            // Preserve the original delegate type in attribute, so it can be retrieved later in code.
            methodBuilder.SetCustomAttribute(
                new CustomAttributeBuilder(
                    typeof(ProxiedDelegateTypeAttribute).GetConstructors().Single(),
                    new object[] {delegateType}));

            return typeBuilder.CreateTypeInfo().AsType();
        }
    }
}