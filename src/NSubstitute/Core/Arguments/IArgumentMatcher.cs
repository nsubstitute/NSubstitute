namespace NSubstitute.Core.Arguments;

/// <summary>
/// Provides a specification for arguments.
/// Can implement <see cref="IDescribeNonMatches" /> to give descriptions when arguments do not match.
/// Can implement <see cref="IDescribeSpecification"/> to give descriptions of expected arguments (otherwise
/// `ToString()` will be used for descriptions).
/// </summary>
public interface IArgumentMatcher
{
    /// <summary>
    /// Checks whether the <paramref name="argument"/> satisfies the condition of the matcher.
    /// If this throws an exception the argument will be treated as non-matching.
    /// </summary>
    bool IsSatisfiedBy(object? argument);
}

/// <summary>
/// Provides a specification for arguments.
/// Can implement <see cref="IDescribeNonMatches" /> to give descriptions when arguments do not match.
/// Can implement <see cref="IDescribeSpecification"/> to give descriptions of expected arguments (otherwise
/// `ToString()` will be used for descriptions).
/// </summary>
/// <typeparam name="T">Matches arguments of type <typeparamref name="T"/> or compatible type.</typeparam>
public interface IArgumentMatcher<T>
{
    /// <summary>
    /// Checks whether the <paramref name="argument"/> satisfies the condition of the matcher.
    /// If this throws an exception the argument will be treated as non-matching.
    /// </summary>
    bool IsSatisfiedBy(T? argument);
}