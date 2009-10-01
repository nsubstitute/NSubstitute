namespace NSubstitute
{
    public interface IInvocationResults
    {
        void SetResult<T>(IInvocation invocation, T valueToReturn);
        object GetResult(IInvocation invocation);
    }
}