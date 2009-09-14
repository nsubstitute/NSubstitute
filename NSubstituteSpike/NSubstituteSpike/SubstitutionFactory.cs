using LinFu.Proxy;
using LinFu.Proxy.Interfaces;

namespace NSubstituteSpike
{
    public class SubstitutionFactory
    {
        public T Create<T>()
        {
            var proxyFactory = new ProxyFactory();
            var proxy = proxyFactory.CreateDuck<T>();
            ((IProxy) proxy).Interceptor = new SubstituteInterceptor();
            return proxy;
        }
    }
}