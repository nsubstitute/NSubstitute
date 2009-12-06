namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        readonly CastleProxyGeneratorWrapper _proxyGeneratorWrapper;
        readonly CastleInterceptorFactory _interceptorFactory;
        
        public CastleDynamicProxyFactory(CastleProxyGeneratorWrapper proxyGeneratorWrapper, CastleInterceptorFactory interceptorFactory)
        {
            _proxyGeneratorWrapper = proxyGeneratorWrapper;
            _interceptorFactory = interceptorFactory;
        }

        public T GenerateProxy<T>(IInvocationHandler invocationHandler) where T : class
        {
            var interceptor = _interceptorFactory.CreateForwardingInterceptor(invocationHandler);
            if (typeof(T).IsInterface)
            {
                return _proxyGeneratorWrapper.CreateProxyForInterface<T>(interceptor);
            }
            return _proxyGeneratorWrapper.CreateProxyForClass<T>(interceptor);
        }
    }
}