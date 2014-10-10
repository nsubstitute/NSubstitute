using System;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
	using System.Collections.Generic;

	public class CastleDynamicProxyFactory : IProxyFactory
    {
        readonly ProxyGenerator _proxyGenerator;

        public CastleDynamicProxyFactory()
        {
            ConfigureDynamicProxyToAvoidReplicatingProblematicAttributes();

            _proxyGenerator = new ProxyGenerator();
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments, object[] mixins)
        {
            VerifyClassHasNotBeenPassedAsAnAdditionalInterface(additionalInterfaces);

            var interceptor = new CastleForwardingInterceptor(new CastleInvocationMapper(), callRouter);
            var proxyGenerationOptions = GetOptionsToMixinCallRouter(new [] {callRouter}.Concat(mixins).ToList());
            var proxy = CreateProxyUsingCastleProxyGenerator(typeToProxy, additionalInterfaces, constructorArguments, interceptor, proxyGenerationOptions);
            interceptor.StartIntercepting();
            return proxy;
        }

        private object CreateProxyUsingCastleProxyGenerator(Type typeToProxy, Type[] additionalInterfaces,
                                                            object[] constructorArguments,
                                                            IInterceptor interceptor,
                                                            ProxyGenerationOptions proxyGenerationOptions)
        {
            if (typeToProxy.IsInterface)
            {
                VerifyNoConstructorArgumentsGivenForInterface(constructorArguments);
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, additionalInterfaces, proxyGenerationOptions, interceptor);
            }
            return _proxyGenerator.CreateClassProxy(typeToProxy, additionalInterfaces, proxyGenerationOptions, constructorArguments, interceptor);
        }

        private ProxyGenerationOptions GetOptionsToMixinCallRouter(IList<object> mixins)
        {
			var options = new ProxyGenerationOptions(new AllMethodsExceptMixinsCallsHook(mixins));
			foreach (var mixin in mixins)
	        {
				options.AddMixinInstance(mixin);
			}
            return options;
        }

        private class AllMethodsExceptMixinsCallsHook : AllMethodsHook
        {
			private readonly HashSet<Type> mixinTypes;

	        public AllMethodsExceptMixinsCallsHook(IEnumerable<object> mixins)
	        {
		        this.mixinTypes = new HashSet<Type>(mixins.Select(m => m.GetType()).SelectMany(GetAllTypes));
	        }

	        public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return IsNotMixinMethod(methodInfo)
                    && IsNotBaseObjectMethod(methodInfo)
                    && base.ShouldInterceptMethod(type, methodInfo);
            }

            private bool IsNotMixinMethod(MethodInfo methodInfo)
            {
                return !mixinTypes.Contains(methodInfo.DeclaringType);
            }

            private static bool IsNotBaseObjectMethod(MethodInfo methodInfo)
            {
                return methodInfo.GetBaseDefinition().DeclaringType != typeof (object);
            }

	        private static IEnumerable<Type> GetAllTypes(Type type)
			{
				foreach (var i in type.GetInterfaces())
				{
					yield return i;
				}

				while (type != typeof(object))
				{
					yield return type;
					type = type.BaseType;
				}
	        }
        }

        private void VerifyNoConstructorArgumentsGivenForInterface(object[] constructorArguments)
        {
            if (constructorArguments != null && constructorArguments.Length > 0)
            {
                throw new SubstituteException("Can not provide constructor arguments when substituting for an interface.");
            }
        }

        private void VerifyClassHasNotBeenPassedAsAnAdditionalInterface(Type[] additionalInterfaces)
        {
            if (additionalInterfaces != null && additionalInterfaces.Any(x => x.IsClass))
            {
                throw new SubstituteException("Can not substitute for multiple classes. To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.");
            }
        }

        private static void ConfigureDynamicProxyToAvoidReplicatingProblematicAttributes()
        {
#pragma warning disable 618
            AttributesToAvoidReplicating.Add<SecurityPermissionAttribute>();
#pragma warning restore 618

            AttributesToAvoidReplicating.Add<System.ServiceModel.ServiceContractAttribute>();

            AttributesToAvoidReplicating.Add<ReflectionPermissionAttribute>();
            AttributesToAvoidReplicating.Add<PermissionSetAttribute>();
            AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.MarshalAsAttribute>();
#if (NET4 || NET45)
            AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.TypeIdentifierAttribute>();
#endif
            AttributesToAvoidReplicating.Add<UIPermissionAttribute>();
        }
    }
}