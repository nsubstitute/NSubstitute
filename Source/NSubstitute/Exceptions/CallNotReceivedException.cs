using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CallNotReceivedException : SubstituteException
    {
        public CallNotReceivedException() { }
        public CallNotReceivedException(string message) : base(message) { }
        public CallNotReceivedException(string message, Exception innerException) : base(message, innerException) { }
#if !SILVERLIGHT
        protected CallNotReceivedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}