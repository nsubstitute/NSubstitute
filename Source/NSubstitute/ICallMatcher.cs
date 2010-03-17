namespace NSubstitute
{
    public interface ICallMatcher
    {
        bool IsMatch(ICall first, ICall second);
    }
}