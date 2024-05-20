namespace NSubstitute.Core.Arguments;

public class EqualsArgumentMatcher(object? value) : IArgumentMatcher
{
    public override string ToString() => ArgumentFormatter.Default.Format(value, false);

    public bool IsSatisfiedBy(object? argument) => EqualityComparer<object>.Default.Equals(value, argument);
}