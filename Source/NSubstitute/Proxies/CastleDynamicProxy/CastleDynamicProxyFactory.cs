using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        readonly ProxyGenerator _proxyGenerator;
        readonly CastleInterceptorFactory _interceptorFactory;
        readonly IgnoreCallHandlerCallsHook _ignoreCallHandlerCallsHook;

        public CastleDynamicProxyFactory(ProxyGenerator proxyGenerator, CastleInterceptorFactory interceptorFactory)
        {
            _proxyGenerator = proxyGenerator;
            _interceptorFactory = interceptorFactory;            
            _ignoreCallHandlerCallsHook = new IgnoreCallHandlerCallsHook();
        }

        public T GenerateProxy<T>(ICallHandler callHandler) where T : class
        {
            var interceptor = _interceptorFactory.CreateForwardingInterceptor(callHandler);
            var proxyGenerationOptions = GetOptionsToMixinInvocationHandler(callHandler);
            if (typeof(T).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(proxyGenerationOptions, interceptor);
            }
            return _proxyGenerator.CreateClassProxy<T>(proxyGenerationOptions, interceptor);
        }

        private ProxyGenerationOptions GetOptionsToMixinInvocationHandler(ICallHandler callHandler)
        {
            var options = new ProxyGenerationOptions(_ignoreCallHandlerCallsHook);
            options.AddMixinInstance(callHandler);
            return options;
        }

        private class IgnoreCallHandlerCallsHook : IProxyGenerationHook
        {
            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return type != typeof (ICallHandler);
            }

            public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo) {}

            public void MethodsInspected() {}
        }
    }
}