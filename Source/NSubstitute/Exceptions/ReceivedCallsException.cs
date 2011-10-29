using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class ReceivedCallsException : SubstituteException
    {
        public ReceivedCallsException() { }
        public ReceivedCallsException(string message) : base(message) { }
        public ReceivedCallsException(string message, Exception innerException) : base(message, innerException) { }
#if !SILVERLIGHT
        protected ReceivedCallsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}