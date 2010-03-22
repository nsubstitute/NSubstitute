namespace NSubstitute
{
    public interface ICallMatcher
    {
        bool IsMatch(ICall call, ICallSpecification callSpecification);
    }
}