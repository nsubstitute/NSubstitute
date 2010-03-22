namespace NSubstitute
{
    public interface IArgumentMatcher
    {
        bool Matches(object argument);
    }
}