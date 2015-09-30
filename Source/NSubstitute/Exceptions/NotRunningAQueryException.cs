using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    [Serializable]
    public class NotRunningAQueryException : SubstituteException
    {
        public NotRunningAQueryException() { }
        protected NotRunningAQueryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
