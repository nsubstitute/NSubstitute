using System;
#if !DNXCORE50
using System.Runtime.Serialization;
#endif


namespace NSubstitute.Exceptions
{
    public class CannotCreateEventArgsException : SubstituteException
    {
        public CannotCreateEventArgsException() { }
        public CannotCreateEventArgsException(string message) : base(message) { }
        public CannotCreateEventArgsException(string message, Exception innerException) : base(message, innerException) { }
#if !DNXCORE50
        protected CannotCreateEventArgsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}