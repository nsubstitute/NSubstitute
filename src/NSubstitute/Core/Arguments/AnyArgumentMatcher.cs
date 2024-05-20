namespace NSubstitute.Core.Arguments;

public class AnyArgumentMatcher(Type typeArgMustBeCompatibleWith) : IArgumentMatcher
{
    public override string ToString() => "any " + typeArgMustBeCompatibleWith.GetNonMangledTypeName();

    public bool IsSatisfiedBy(object? argument) => argument.IsCompatibleWith(typeArgMustBeCompatibleWith);
}