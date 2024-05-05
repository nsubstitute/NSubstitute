namespace NSubstitute.Core.Arguments;

public class EqualsArgumentMatcher(object? value) : IArgumentMatcher
{
    public override string ToString() => ArgumentFormatter.Default.Format(value, false);

    public bool IsSatisfiedBy(object? argument) => EqualityComparer<object>.Default.Equals(value, argument);
}

public class TypedEqualsArgumentMatcher<T>(T? value) : IArgumentMatcher<T>
{
    public override string ToString() => ArgumentFormatter.Default.Format(value, false);

    public bool IsSatisfiedBy(T? argument) => EqualityComparer<T>.Default.Equals(argument, value);
}