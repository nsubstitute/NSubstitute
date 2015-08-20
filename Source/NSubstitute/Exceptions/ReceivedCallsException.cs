using System;
#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class ReceivedCallsException : SubstituteException
    {
        public ReceivedCallsException() { }
        public ReceivedCallsException(string message) : base(message) { }
        public ReceivedCallsException(string message, Exception innerException) : base(message, innerException) { }
#if !DNXCORE50
        protected ReceivedCallsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}