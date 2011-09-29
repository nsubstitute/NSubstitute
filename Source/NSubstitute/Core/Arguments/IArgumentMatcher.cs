namespace NSubstitute.Core.Arguments
{
    public interface IArgumentMatcher
    {
        bool IsSatisfiedBy(object argument);
    }
}