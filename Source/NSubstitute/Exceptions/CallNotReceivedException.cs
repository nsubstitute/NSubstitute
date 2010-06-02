using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CallNotReceivedException : Exception
    {
        public CallNotReceivedException() { }
        public CallNotReceivedException(string message) : base(message) { }
        public CallNotReceivedException(string message, Exception innerException) : base(message, innerException) { }
        protected CallNotReceivedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}