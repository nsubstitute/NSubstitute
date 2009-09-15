using System;
using Castle.DynamicProxy;

namespace NSubstituteSpike
{
    public class CastleSubstitutionFactory : ISubstitutionFactory
    {
        public T Create<T>()
        {
            var proxyGenerator = new ProxyGenerator();
            return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(new CastleSubstituteInterceptor());
        }
    }
}