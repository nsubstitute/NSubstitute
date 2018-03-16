namespace NSubstitute.Core
{
    public interface IConfigureCall
    {
        ConfiguredCall SetResultForLastCall(IReturn valueToReturn, MatchArgs matchArgs, IPendingSpecification pendingSpecification);
        void SetResultForCall(ICall call, IReturn valueToReturn, MatchArgs matchArgs);
    }
}