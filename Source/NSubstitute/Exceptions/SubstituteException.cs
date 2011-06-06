using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class SubstituteException : Exception
    {
        public SubstituteException() : this("") { }
        public SubstituteException(string message) : this(message, null) { }
        public SubstituteException(string message, Exception innerException) : base(message, innerException) { }
#if !SILVERLIGHT        
        protected SubstituteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}