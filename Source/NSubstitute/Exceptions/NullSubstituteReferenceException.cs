using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    [Serializable]
    public class NullSubstituteReferenceException : NotASubstituteException 
    {
        public NullSubstituteReferenceException() { }
        protected NullSubstituteReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
