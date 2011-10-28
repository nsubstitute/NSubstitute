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
            return ArgumentIsCompatibleWithType(argument);
        }

        private bool ArgumentIsCompatibleWithType(object argument) 
        {
            var requiredType = (_typeArgMustBeCompatibleWith.IsByRef) ? _typeArgMustBeCompatibleWith.GetElementType() : _typeArgMustBeCompatibleWith;
            return argument == null ? TypeCanBeNull(requiredType) : requiredType.IsAssignableFrom(argument.GetType());
        }

        private bool TypeCanBeNull(Type type) { return !type.IsValueType; }
    }
}