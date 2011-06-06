using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CannotCreateEventArgsException : SubstituteException
    {
        public CannotCreateEventArgsException() { }
        public CannotCreateEventArgsException(string message) : base(message) { }
        public CannotCreateEventArgsException(string message, Exception innerException) : base(message, innerException) { }
#if !SILVERLIGHT
        protected CannotCreateEventArgsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}