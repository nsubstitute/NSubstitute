using System;

namespace NSubstitute.Exceptions
{
    public class SubstituteException : Exception
    {
        public SubstituteException()
            : this("")
        {
        }

        public SubstituteException(string message)
            : base(message)
        {
        }
    }
}