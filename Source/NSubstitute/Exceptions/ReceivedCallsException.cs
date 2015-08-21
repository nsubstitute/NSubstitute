using System;
#if NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class ReceivedCallsException : SubstituteException
    {
        public ReceivedCallsException() { }
        public ReceivedCallsException(string message) : base(message) { }
        public ReceivedCallsException(string message, Exception innerException) : base(message, innerException) { }
#if NET4 || NET45
        protected ReceivedCallsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}