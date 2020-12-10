namespace NSubstitute.Core
{
    public interface IDescribeNonMatches
    {
        /// <summary>
        /// Describes how the <paramref name="argument" /> does not match the condition specified by this class, or <see cref="string.Empty"/>
        /// if a detailed description can not be provided for the argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns>Description of the non-match, or <see cref="string.Empty" /> if no description can be provided.</returns>
        string DescribeFor(object? argument);
    }
}