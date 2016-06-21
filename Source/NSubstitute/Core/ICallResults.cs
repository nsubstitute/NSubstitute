namespace NSubstitute.Core
{
    public interface ICallResults
    {
        void SetResult(ICallSpecification callSpecification, IReturn result);
        bool HasResultFor(ICall call);
        object GetResult(ICall call);
	    void Clear();
    }
}