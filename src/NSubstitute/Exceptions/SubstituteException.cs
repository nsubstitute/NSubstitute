using System;

namespace NSubstitute.Exceptions
{
    public class SubstituteException : Exception
    {
        public SubstituteException() : this("") { }
        public SubstituteException(string message) : this(message, null) { }
        public SubstituteException(string message, Exception? innerException) : base(message, innerException) { }
    }
}
