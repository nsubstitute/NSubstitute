using NSubstitute.Core.Arguments;
using System.Linq.Expressions;

namespace NSubstitute;

/// <summary>
/// Argument matcher allowing a match predicate and optional action to be called for each match to be specified serparately.
/// </summary>
public class Match<T>
{
    private Expression<Predicate<T>> predicate;
    private Action<T> useArgument;

    internal Match(Expression<Predicate<T>> predicate, Action<T> useArgument)
    {
        this.predicate = predicate;
        this.useArgument = useArgument;
    }

    /// <summary>
    /// The <paramref name="useArgument"/> function to be invoked
    /// for each matching call made to the substitute.
    /// </summary>
    public Match<T> AndDo(Action<T> useArgument) => new Match<T>(predicate, x => { this.useArgument(x); useArgument(x); });

    public static implicit operator T? (Match<T?> match)
    {
        return ArgumentMatcher.Enqueue<T>(
            new ExpressionArgumentMatcher<T>(match.predicate),
            x => match.useArgument((T?)x)
        );
    }
}

public static class Match
{
    /// <summary>
    /// Match argument that satisfies <paramref name="predicate"/>.
    /// If the <paramref name="predicate"/> throws an exception for an argument it will be treated as non-matching.
    /// </summary>
    public static Match<T> When<T>(Expression<Predicate<T>> predicate) =>
        new Match<T>(predicate, x => { });
}
