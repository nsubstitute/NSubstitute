using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    [Serializable]
    public class ArgumentNotFoundException : SubstituteException
    {
        public ArgumentNotFoundException(string message) : base(message) { }
        protected ArgumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
