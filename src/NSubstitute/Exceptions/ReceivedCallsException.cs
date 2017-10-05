using System;

namespace NSubstitute.Exceptions
{
    public class ReceivedCallsException : SubstituteException
    {
        public ReceivedCallsException() { }
        public ReceivedCallsException(string message) : base(message) { }
        public ReceivedCallsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
