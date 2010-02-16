namespace NSubstitute
{
    public interface IInvocationHandler
    {
        void LastInvocationShouldReturn<T>(T valueToReturn);
        object HandleInvocation(IInvocation invocation);
        void AssertNextCallHasBeenReceived();
    }
}