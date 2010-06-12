using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CallReceivedException : Exception
    {
        public CallReceivedException() { }
        public CallReceivedException(string message) : base(message) { }
        public CallReceivedException(string message, Exception innerException) : base(message, innerException) { }
        protected CallReceivedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}