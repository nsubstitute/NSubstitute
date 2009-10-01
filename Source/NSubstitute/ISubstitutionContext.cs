namespace NSubstitute
{
    public interface ISubstitutionContext
    {
        void LastInvocationShouldReturn<T>(T value);
        void LastInvocationHandlerInvoked(IInvocationHandler invocationHandler);
        ISubstituteFactory GetSubstituteFactory();
    }
}