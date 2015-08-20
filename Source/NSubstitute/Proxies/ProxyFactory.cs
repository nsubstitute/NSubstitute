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

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments, object[] mixins)
        {
            var isDelegate = typeToProxy.IsSubclassOf(typeof(Delegate));
            return isDelegate 
                ? _delegateFactory.GenerateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments, mixins)
                : _dynamicProxyFactory.GenerateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments, mixins);
        }
    }
}