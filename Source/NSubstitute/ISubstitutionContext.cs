namespace NSubstitute
{
    public interface ISubstitutionContext
    {
        void LastInvocationShouldReturn<T>(T value);
        void LastInvocationHandlerInvoked(IInvocationHandler _invocationHandler);
    }
}