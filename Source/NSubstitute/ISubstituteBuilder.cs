namespace NSubstitute
{
    public interface ISubstituteBuilder
    {
        ISubstituteInterceptor CreateInterceptor(IInvocationHandler invocationHandler);
        T GenerateProxy<T>(ISubstituteInterceptor interceptor);
    }
}