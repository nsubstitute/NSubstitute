namespace NSubstitute.Core
{
    public interface ICallSpecification
    {
        bool IsSatisfiedBy(ICall call);
        string Format(ICallFormatter callFormatter);
        string HowDifferentFrom(ICall call, ICallFormatter formatter);
    }
}