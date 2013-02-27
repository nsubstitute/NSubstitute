namespace NSubstitute.Core
{
    public interface IGetCallSpec
    {
        ICallSpecification FromLastCall(MatchArgs matchArgs);
        ICallSpecification FromExistingSpec(ICallSpecification spec, MatchArgs matchArgs);
        ICallSpecification FromCall(ICall call, MatchArgs matchArgs);
    }
}