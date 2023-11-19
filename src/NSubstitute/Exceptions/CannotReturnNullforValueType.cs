using System;

namespace NSubstitute.Exceptions
{
    public class CannotReturnNullForValueType : SubstituteException
    {
        private const string Description =
            "Cannot return null for {0} because it is a value type. " +
            "If you want to return the default value for this type use \"default({0})\".";

        public CannotReturnNullForValueType(Type valueType) : base(string.Format(Description, valueType.Name)) {}
    }
}
