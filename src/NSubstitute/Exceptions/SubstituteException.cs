using System;
#if NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET45
    [Serializable]
#endif
    public class SubstituteException : Exception
    {
        public SubstituteException() : this("") { }
        public SubstituteException(string message) : this(message, null) { }
        public SubstituteException(string message, Exception innerException) : base(message, innerException) { }
#if NET45
        protected SubstituteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
