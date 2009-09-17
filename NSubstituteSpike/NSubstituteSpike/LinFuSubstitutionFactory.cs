using LinFu.Proxy;
using LinFu.Proxy.Interfaces;

namespace NSubstituteSpike
{
    public class LinFuSubstitutionFactory : ISubstitutionFactory
    {
        public T Create<T>()
        {
            var proxyFactory = new ProxyFactory();
            return proxyFactory.CreateProxy<T>(new LinFuSubstituteInterceptor());
        }
    }
}