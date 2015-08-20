using System;
#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class CannotReturnNullForValueType : SubstituteException
    {
        const string Description = "Cannot return null for {0} because it is a value type. If you want to return the default value for this type use \"default({0})\".";
        public CannotReturnNullForValueType(Type valueType) : base(string.Format(Description, valueType.Name)) {}
#if !DNXCORE50
        protected CannotReturnNullForValueType(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}