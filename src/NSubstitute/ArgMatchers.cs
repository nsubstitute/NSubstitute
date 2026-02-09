using NSubstitute.Core.Arguments;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute;

/// <summary>
/// Argument matchers for use with <see cref="Arg.Is{T}(IArgumentMatcher{T})"/>.
/// </summary>
public static class ArgMatchers
{
    public static IArgumentMatcher<T> EqualTo<T>(T value) => new TypedEqualsArgumentMatcher<T>(value);

    public static IArgumentMatcher Any<T>() => new AnyArgumentMatcher(typeof(T));


#if NET6_0_OR_GREATER
    /// <summary>
    /// Match argument that satisfies <paramref name="predicate"/>.
    /// If the <paramref name="predicate"/> throws an exception for an argument it will be treated as non-matching.
    /// </summary>
    public static IArgumentMatcher<T> Matching<T>(
        Predicate<T> predicate,
        [System.Runtime.CompilerServices.CallerArgumentExpression("predicate")]
        string predicateDescription = ""
    ) =>
        new PredicateArgumentMatcher<T>(predicate, predicateDescription);

    // See https://github.com/nsubstitute/NSubstitute/issues/822
    private class PredicateArgumentMatcher<T>(Predicate<T> predicate, string predicateDescription) : IArgumentMatcher<T>
    {
        public bool IsSatisfiedBy(T argument) => predicate(argument!);

        public override string ToString() => predicateDescription;
    }
#endif
}