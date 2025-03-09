namespace NSubstitute.Core.Arguments;

internal sealed class AnyArgumentMatcher(Type typeArgMustBeCompatibleWith) : IArgumentMatcher
{
    public override string ToString() => "any " + typeArgMustBeCompatibleWith.GetNonMangledTypeName();

    public bool IsSatisfiedBy(object? argument) => argument.IsCompatibleWith(typeArgMustBeCompatibleWith);
}