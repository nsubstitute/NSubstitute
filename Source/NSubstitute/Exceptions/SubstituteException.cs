using System;
#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class SubstituteException : Exception
    {
        public SubstituteException() : this("") { }
        public SubstituteException(string message) : this(message, null) { }
        public SubstituteException(string message, Exception innerException) : base(message, innerException) { }
#if !DNXCORE50
        protected SubstituteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}