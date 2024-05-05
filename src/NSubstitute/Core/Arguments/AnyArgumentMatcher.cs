namespace NSubstitute.Core.Arguments;

public class AnyArgumentMatcher(Type typeArgMustBeCompatibleWith)
    : IArgumentMatcher, IDescribeSpecification, IDescribeNonMatches
{
    public override string ToString() => "any " + typeArgMustBeCompatibleWith.GetNonMangledTypeName();

    public bool IsSatisfiedBy(object? argument) => argument.IsCompatibleWith(typeArgMustBeCompatibleWith);

    public string DescribeFor(object? argument) =>
        argument?.GetType().GetNonMangledTypeName() ?? "<null>" + " is not a " + typeArgMustBeCompatibleWith.GetNonMangledTypeName();

    public string DescribeSpecification() => ToString();
}
