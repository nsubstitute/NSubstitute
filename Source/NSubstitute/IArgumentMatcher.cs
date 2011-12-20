namespace NSubstitute
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