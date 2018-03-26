namespace NSubstitute.Core
{
    public interface IConfigureCall
    {
        ConfiguredCall SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo);
        void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs);
    }
}