using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class ReceivedCallsException : SubstituteException
    {
        public ReceivedCallsException() { }
        public ReceivedCallsException(string message) : base(message) { }
        public ReceivedCallsException(string message, Exception innerException) : base(message, innerException) { }
#if NET35 || NET4 || NET45
        protected ReceivedCallsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
