namespace NSubstitute
{
    public interface IProxyFactory
    {
        T GenerateProxy<T>(ICallRouter callRouter) where T : class;
    }
}