namespace NSubstitute
{
    public interface ICallMatcher
    {
        bool IsMatch(ICall first, ICall second);
        bool IsMatch(ICall call, ICallSpecification callSpecification);
    }
}