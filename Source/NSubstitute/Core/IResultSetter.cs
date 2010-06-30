namespace NSubstitute.Core
{
    public interface IResultSetter
    {
        void SetResultForLastCall(IReturn valueToReturn, bool forAnyArguments);
        void SetResultForCall(ICall call, IReturn valueToReturn);
    }
}