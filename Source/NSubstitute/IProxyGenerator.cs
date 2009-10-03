namespace NSubstitute
{
    public interface IProxyGenerator
    {
        T GenerateProxy<T>(IInvocationHandler invocationHandler);
    }
}