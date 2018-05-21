namespace NSubstitute.Core.Arguments
{
    /// <summary>
    /// Provides a specification for arguments for use with <see ctype="Arg.Matches (IArgumentMatcher)" />.
    /// Can additionally implement <see cref="IDescribeNonMatches" /> to give descriptions when arguments do not match.
    /// </summary>
    public interface IArgumentMatcher
    {
        /// <summary>
        /// Checks whether the <paramref name="argument"/> satisfies the condition of the matcher.
        /// If this throws an exception the argument will be treated as non-matching.
        /// </summary>
        bool IsSatisfiedBy(object argument);
    }
}