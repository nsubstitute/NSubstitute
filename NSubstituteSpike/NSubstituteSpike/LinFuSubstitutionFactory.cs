using LinFu.Proxy;
using LinFu.Proxy.Interfaces;

namespace NSubstituteSpike
{
    public class LinFuSubstitutionFactory : ISubstitutionFactory
    {
        public T Create<T>()
        {
            var proxyFactory = new ProxyFactory();
            var proxy = proxyFactory.CreateDuck<T>();
            ((IProxy) proxy).Interceptor = new LinFuSubstituteInterceptor();
            return proxy;
        }
    }
}