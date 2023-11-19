using System;

namespace NSubstitute.Core.Arguments
{
    public class AnyArgumentMatcher : IArgumentMatcher
    {
        private readonly Type _typeArgMustBeCompatibleWith;

        public AnyArgumentMatcher(Type typeArgMustBeCompatibleWith)
        {
            _typeArgMustBeCompatibleWith = typeArgMustBeCompatibleWith;
        }

        public override string ToString() => "any " + _typeArgMustBeCompatibleWith.GetNonMangledTypeName();

        public bool IsSatisfiedBy(object? argument) => argument.IsCompatibleWith(_typeArgMustBeCompatibleWith);
    }
}