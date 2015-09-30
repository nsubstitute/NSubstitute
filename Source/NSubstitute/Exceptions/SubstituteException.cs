using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    [Serializable]
    public class SubstituteException : Exception
    {
        public SubstituteException() : this("") { }
        public SubstituteException(string message) : this(message, null) { }
        public SubstituteException(string message, Exception innerException) : base(message, innerException) { }
        protected SubstituteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
