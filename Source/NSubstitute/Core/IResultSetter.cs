namespace NSubstitute.Core
{
    public interface IResultSetter
    {
        void SetResultForLastCall(IReturn valueToReturn);
        void SetResultForCall(ICall call, IReturn valueToReturn);
    }
}