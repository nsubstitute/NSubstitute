namespace NSubstitute.Core
{
    public interface ICallResults
    {
        void SetResult<T>(ICallSpecification callSpecification, T valueToReturn);
        object GetResult(ICall call);
        object GetDefaultResultFor(ICall call);
    }
}