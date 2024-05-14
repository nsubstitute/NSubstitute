namespace NSubstitute.Core;

/// <summary>
/// A type that can describe the required conditions to meet a specification.
/// Use in conjunction with <see cref="NSubstitute.Core.Arguments.IArgumentMatcher"/> to provide information about
/// what it requires to match an argument.
/// </summary>
public interface IDescribeSpecification
{
    /// <summary>
    /// A concise description of the conditions required to match this specification, or <see cref="string.Empty"/>
    /// if a detailed description can not be provided.
    /// </summary>
    /// <returns>Description of the specification, or <see cref="string.Empty" /> if no description can be provided.</returns>
    string DescribeSpecification();
}
