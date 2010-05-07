namespace NSubstitute.Core
{
    public interface ICallSpecificationFactory
    {
        ICallSpecification CreateFrom(ICall call);
    }
}