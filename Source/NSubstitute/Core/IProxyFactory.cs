using System;

namespace NSubstitute.Core
{
    public interface IProxyFactory
    {
        T GenerateProxy<T>(ICallRouter callRouter) where T : class;
        object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments);
    }
}