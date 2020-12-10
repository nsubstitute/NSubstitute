using System;

namespace NSubstitute.Exceptions
{

    public class SubstituteInternalException : SubstituteException
    {
        public SubstituteInternalException() : this("") { }
        public SubstituteInternalException(string message) : this(message, null) { }
        public SubstituteInternalException(string message, Exception? innerException)
            : base("Please report this exception at https://github.com/nsubstitute/NSubstitute/issues: \n\n" + message,
                innerException) { }
    }
}