using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        readonly ProxyGenerator _proxyGenerator;
        readonly AllMethodsExceptCallRouterCallsHook _allMethodsExceptCallRouterCallsHook;

        public CastleDynamicProxyFactory()
        {
            _proxyGenerator = new ProxyGenerator();
            _allMethodsExceptCallRouterCallsHook = new AllMethodsExceptCallRouterCallsHook();
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments)
        {
            VerifyClassHasNotBeenPassedAsAnAdditionalInterface(additionalInterfaces);

            var interceptor = new CastleForwardingInterceptor(new CastleInvocationMapper(), callRouter);
            var proxyGenerationOptions = GetOptionsToMixinCallRouter(callRouter);
            var proxy = CreateProxyUsingCastleProxyGenerator(typeToProxy, additionalInterfaces, constructorArguments, interceptor, proxyGenerationOptions);
            interceptor.StartIntercepting();
            return proxy;
        }

        private object CreateProxyUsingCastleProxyGenerator(Type typeToProxy, Type[] additionalInterfaces,
                                                            object[] constructorArguments,
                                                            IInterceptor interceptor,
                                                            ProxyGenerationOptions proxyGenerationOptions)
        {
            if (typeToProxy.GetTypeInfo().IsInterface)
            {
                VerifyNoConstructorArgumentsGivenForInterface(constructorArguments);
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, additionalInterfaces, proxyGenerationOptions, interceptor);
            }
            return _proxyGenerator.CreateClassProxy(typeToProxy, additionalInterfaces, proxyGenerationOptions, constructorArguments, interceptor);
        }

        private ProxyGenerationOptions GetOptionsToMixinCallRouter(ICallRouter callRouter)
        {
            var options = new ProxyGenerationOptions(_allMethodsExceptCallRouterCallsHook);
            options.AddMixinInstance(callRouter);
            return options;
        }

        private class AllMethodsExceptCallRouterCallsHook : AllMethodsHook
        {
            public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return IsNotCallRouterMethod(methodInfo)
                    && IsNotBaseObjectMethod(methodInfo)
                    && base.ShouldInterceptMethod(type, methodInfo);
            }

            private static bool IsNotCallRouterMethod(MethodInfo methodInfo)
            {
                return methodInfo.DeclaringType != typeof(ICallRouter);
            }

            private static bool IsNotBaseObjectMethod(MethodInfo methodInfo)
            {
                return methodInfo.GetBaseDefinition().DeclaringType != typeof (object);
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
            if (additionalInterfaces != null && additionalInterfaces.Any(x => x.GetTypeInfo().IsClass))
            {
                throw new SubstituteException("Can not substitute for multiple classes. To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.");
            }
        }
    }
}