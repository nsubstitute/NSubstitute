namespace NSubstitute
{
    public interface IProxyFactory
    {
        T GenerateProxy<T>(ICallHandler callHandler) where T : class;
    }
}