using System;

namespace NSubstitute.Exceptions
{
    public class AmbiguousArgumentsException : SubstituteException
    {
        public static string SpecifyAllArguments = "Cannot determine argument specifications to use." + Environment.NewLine +
                                                    "Please use specifications for all arguments of the same type.";
        public AmbiguousArgumentsException() : this(SpecifyAllArguments) { }
        public AmbiguousArgumentsException(string message) : base(message) { }
    }
}
