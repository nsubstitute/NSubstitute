namespace NSubstitute.Core.Arguments;

public class ReferenceArgumentMatcher<T>(T value) : IArgumentMatcher<T>, IDescribeNonMatches
    where T : class
{
    public override string ToString() => ArgumentFormatter.Default.Format(value, false);

    public string DescribeFor(object? argument)
    {
        var expected = ArgumentFormatter.Default.Format(value, false);
        var actual = ArgumentFormatter.Default.Format(argument, false);

        return $"Reference of {expected} does not match reference of {actual}.";
    }

    public bool IsSatisfiedBy(T? argument) => ReferenceEquals(argument, value);
}