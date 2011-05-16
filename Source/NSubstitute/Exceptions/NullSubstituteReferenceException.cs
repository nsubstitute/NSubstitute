using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class NullSubstituteReferenceException : NotASubstituteException 
    {
        public NullSubstituteReferenceException() { }
        protected NullSubstituteReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}