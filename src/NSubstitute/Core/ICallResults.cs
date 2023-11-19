namespace NSubstitute.Core
{
    public interface ICallResults
    {
        void SetResult(ICallSpecification callSpecification, IReturn result);
        bool TryGetResult(ICall call, out object? result);
	    void Clear();
    }
}