namespace NSubstitute.Core;

public interface IProxyFactory
{
    object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[]? additionalInterfaces, bool isPartial, object?[]? constructorArguments);
}