namespace NSubstitute.Core
{
    public interface IProxyFactory
    {
        T GenerateProxy<T>(ICallRouter callRouter) where T : class;
    }
}