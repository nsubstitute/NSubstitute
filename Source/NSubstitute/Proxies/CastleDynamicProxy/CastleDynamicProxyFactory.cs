using System;
using System.Reflection;
using Castle.DynamicProxy;

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

        public T GenerateProxy<T>(ICallRouter callRouter) where T : class
        {
            var interceptor = _interceptorFactory.CreateForwardingInterceptor(callRouter);
            var proxyGenerationOptions = GetOptionsToMixinCallRouter(callRouter);
            if (typeof(T).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(proxyGenerationOptions, interceptor);
            }
            return _proxyGenerator.CreateClassProxy<T>(proxyGenerationOptions, interceptor);
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