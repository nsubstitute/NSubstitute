using System;
using NSubstitute.Core;

namespace NSubstitute.Proxies
{
    public class ProxyFactory : IProxyFactory
    {
        private readonly IProxyFactory _delegateFactory;
        private readonly IProxyFactory _dynamicProxyFactory;

        public ProxyFactory(IProxyFactory delegateFactory, IProxyFactory dynamicProxyFactory)
        {
            _delegateFactory = delegateFactory;
            _dynamicProxyFactory = dynamicProxyFactory;
        }

        public T GenerateProxy<T>(ICallRouter callRouter) where T : class
        {
            var isDelegate = typeof(T).IsSubclassOf(typeof(Delegate));
            return isDelegate 
                ? _delegateFactory.GenerateProxy<T>(callRouter) 
                : _dynamicProxyFactory.GenerateProxy<T>(callRouter);
        }
    }
}