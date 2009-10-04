using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleProxyGeneratorWrapper
    {
        readonly ProxyGenerator _proxyGenerator;

        public CastleProxyGeneratorWrapper()
        {
            _proxyGenerator = new ProxyGenerator();
        }

        public virtual T CreateProxyForInterface<T>(IInterceptor interceptor)
        {
            return _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
        }

        public virtual T CreateProxyForClass<T>(IInterceptor interceptor)
        {
            return (T) _proxyGenerator.CreateClassProxy(typeof(T), interceptor);
        }
    }
}