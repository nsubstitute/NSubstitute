namespace NSubstitute.Core
{
    public interface IResultSetter
    {
        void SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs);
        void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs);
    }
}