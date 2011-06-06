using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CallReceivedException : SubstituteException
    {
        public CallReceivedException() { }
        public CallReceivedException(string message) : base(message) { }
        public CallReceivedException(string message, Exception innerException) : base(message, innerException) { }
#if !SILVERLIGHT
        protected CallReceivedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}