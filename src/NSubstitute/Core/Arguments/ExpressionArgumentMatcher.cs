using System.Linq.Expressions;

namespace NSubstitute.Core.Arguments;

public class ExpressionArgumentMatcher<T>(Expression<Predicate<T?>> predicate) : IArgumentMatcher
{
    private readonly string _predicateDescription = predicate.ToString();
    private readonly Predicate<T?> _predicate = predicate.Compile();

    public bool IsSatisfiedBy(object? argument) => _predicate((T?)argument!);

    public override string ToString() => _predicateDescription;
}