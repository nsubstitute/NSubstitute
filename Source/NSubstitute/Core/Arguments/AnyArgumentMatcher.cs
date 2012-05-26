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
            // fix when argument == null and ForType is nullable
            // see: http://msdn.microsoft.com/en-us/library/ms366789.aspx
            if (argument == null && (_typeArgMustBeCompatibleWith.IsGenericType && _typeArgMustBeCompatibleWith.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return true;
            }

            return argument.IsCompatibleWith(_typeArgMustBeCompatibleWith);
        }
    }
}