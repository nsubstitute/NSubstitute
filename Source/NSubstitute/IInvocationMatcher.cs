namespace NSubstitute
{
    public interface IInvocationMatcher
    {
        bool IsMatch(IInvocation first, IInvocation second);
    }
}