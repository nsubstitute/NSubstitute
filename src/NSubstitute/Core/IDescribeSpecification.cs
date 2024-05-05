namespace NSubstitute.Core;

/// <summary>
/// A type that can describe the required conditions to meet a specification.
/// Use in conjunction with <see cref="NSubstitute.Core.Arguments.IArgumentMatcher"/> to provide information about
/// what it requires to match an argument.
/// </summary>
public interface IDescribeSpecification
{

    /// <summary>
    /// A concise description of the conditions required to match this specification.
    /// </summary>
    /// <returns></returns>
    string DescribeSpecification();
}
