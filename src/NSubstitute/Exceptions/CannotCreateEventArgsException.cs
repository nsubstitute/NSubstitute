using System;

namespace NSubstitute.Exceptions
{
    public class CannotCreateEventArgsException : SubstituteException
    {
        public CannotCreateEventArgsException() { }
        public CannotCreateEventArgsException(string message) : base(message) { }
        public CannotCreateEventArgsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
