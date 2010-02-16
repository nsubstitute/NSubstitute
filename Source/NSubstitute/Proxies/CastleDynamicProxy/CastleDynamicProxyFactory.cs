using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        readonly ProxyGenerator _proxyGenerator;
        readonly CastleInterceptorFactory _interceptorFactory;
        readonly IgnoreInvocationHandlerCallsHook _ignoreInvocationHandlerCallsHook;

        public CastleDynamicProxyFactory(ProxyGenerator proxyGenerator, CastleInterceptorFactory interceptorFactory)
        {
            _proxyGenerator = proxyGenerator;
            _interceptorFactory = interceptorFactory;            
            _ignoreInvocationHandlerCallsHook = new IgnoreInvocationHandlerCallsHook();
        }

        public T GenerateProxy<T>(IInvocationHandler invocationHandler) where T : class
        {
            var interceptor = _interceptorFactory.CreateForwardingInterceptor(invocationHandler);
            var proxyGenerationOptions = GetOptionsToMixinInvocationHandler(invocationHandler);
            if (typeof(T).IsInterface)
            {
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(proxyGenerationOptions, interceptor);
            }
            return _proxyGenerator.CreateClassProxy<T>(proxyGenerationOptions, interceptor);
        }

        private ProxyGenerationOptions GetOptionsToMixinInvocationHandler(IInvocationHandler invocationHandler)
        {
            var options = new ProxyGenerationOptions(_ignoreInvocationHandlerCallsHook);
            options.AddMixinInstance(invocationHandler);
            return options;
        }

        private class IgnoreInvocationHandlerCallsHook : IProxyGenerationHook
        {
            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return type != typeof (IInvocationHandler);
            }

            public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo) {}

            public void MethodsInspected() {}
        }
    }
}