namespace NSubstitute.Core
{
    public interface ICallSpecificationFactory
    {
        ICallSpecification CreateFrom(ICall call, MatchArgs matchArgs);
        ICallSpecification CreateFrom(ICallSpecification callSpecification, MatchArgs matchArgs);
    }
}