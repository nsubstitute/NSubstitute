using System;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        readonly ProxyGenerator _proxyGenerator;
        readonly CastleInterceptorFactory _interceptorFactory;
        readonly IgnoreCallRouterCallsHook _ignoreCallRouterCallsHook;

        public CastleDynamicProxyFactory(CastleInterceptorFactory interceptorFactory)
        {
            _proxyGenerator = new ProxyGenerator();
            _interceptorFactory = interceptorFactory;            
            _ignoreCallRouterCallsHook = new IgnoreCallRouterCallsHook();
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments)
        {
            var interceptor = _interceptorFactory.CreateForwardingInterceptor(callRouter);
            var proxyGenerationOptions = GetOptionsToMixinCallRouter(callRouter);
            if (typeToProxy.IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, additionalInterfaces, proxyGenerationOptions, interceptor);
            }
            return _proxyGenerator.CreateClassProxy(typeToProxy, additionalInterfaces, proxyGenerationOptions, constructorArguments, interceptor);

        }

        private ProxyGenerationOptions GetOptionsToMixinCallRouter(ICallRouter callRouter)
        {
            var options = new ProxyGenerationOptions(_ignoreCallRouterCallsHook);
            options.AddMixinInstance(callRouter);
            return options;
        }

        private class IgnoreCallRouterCallsHook : IProxyGenerationHook
        {
            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return type != typeof (ICallRouter);
            }

            public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo) {}

            public void MethodsInspected() {}
        }
    }
}