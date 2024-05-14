using NSubstitute.Core;

namespace NSubstitute.Proxies;

[Obsolete("This class is deprecated and will be removed in future versions of the product.")]
public class ProxyFactory(IProxyFactory delegateFactory, IProxyFactory dynamicProxyFactory) : IProxyFactory
{
    public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[]? additionalInterfaces, object?[]? constructorArguments)
    {
        var isDelegate = typeToProxy.IsDelegate();
        return isDelegate
            ? delegateFactory.GenerateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments)
            : dynamicProxyFactory.GenerateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments);
    }
}