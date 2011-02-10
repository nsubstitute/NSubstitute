using System;

namespace NSubstitute.Exceptions
{
    public class ArgumentMatchingException : SubstituteException
    {
        const string Description = "The \"{0}\" argument matcher threw an exception when testing argument \"{1}\".\n" + 
            "Ensure matchers specified using Arg.Is<T>(Expression<Predicate<T>> predicate) can handle all arguments of type {2}.\n" +
            "See inner exception for more information";

        public ArgumentMatchingException(string matcherDescription, object argument, Type argType, Exception innerException)
            : base(string.Format(Description, matcherDescription, argument ?? "<null>", argType), innerException)
        {
        }
    }
}