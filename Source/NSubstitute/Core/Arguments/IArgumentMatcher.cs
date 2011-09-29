namespace NSubstitute.Core.Arguments
{
    public interface IArgumentMatcher
    {
        bool IsSatisfiedBy(object argument);
    }

    public interface IArgumentMatcher<T>
    {
        bool IsSatisfiedBy(T argument);
    }
}