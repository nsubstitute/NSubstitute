using System;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Proxies
{
    [Obsolete("This class is deprecated and will be removed in future versions of the product.")]
    public class ProxyFactory : IProxyFactory
    {
        private readonly IProxyFactory _delegateFactory;
        private readonly IProxyFactory _dynamicProxyFactory;

        public ProxyFactory(IProxyFactory delegateFactory, IProxyFactory dynamicProxyFactory)
        {
            _delegateFactory = delegateFactory;
            _dynamicProxyFactory = dynamicProxyFactory;
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[]? additionalInterfaces, object?[]? constructorArguments)
        {
            var isDelegate = typeToProxy.IsDelegate();
            return isDelegate 
                ? _delegateFactory.GenerateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments)
                : _dynamicProxyFactory.GenerateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments);
        }
    }
}