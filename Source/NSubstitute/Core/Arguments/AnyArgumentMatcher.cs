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

        public override string ToString() { return "any " + _typeArgMustBeCompatibleWith.Name; }

        public bool IsSatisfiedBy(object argument)
        {
            if (argument == null) return true;
            var specifiedType = (_typeArgMustBeCompatibleWith.IsByRef) ? _typeArgMustBeCompatibleWith.GetElementType() : _typeArgMustBeCompatibleWith;
            return specifiedType.IsAssignableFrom(argument.GetType());
        }
    }
}