namespace NSubstitute.Core
{
    public interface IGetCallSpec
    {
        ICallSpecification FromPendingSpecification(MatchArgs matchArgs);
        ICallSpecification FromExistingSpec(ICallSpecification spec, MatchArgs matchArgs);
        ICallSpecification FromCall(ICall call, MatchArgs matchArgs);
    }
}