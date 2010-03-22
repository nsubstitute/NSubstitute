namespace NSubstitute
{
    public interface ICallSpecificationFactory
    {
        ICallSpecification Create(ICall call);
    }
}