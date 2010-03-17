namespace NSubstitute
{
    public interface ICallResults
    {
        void SetResult<T>(ICall call, T valueToReturn);
        object GetResult(ICall call);
        object GetDefaultResultFor(ICall call);
    }
}