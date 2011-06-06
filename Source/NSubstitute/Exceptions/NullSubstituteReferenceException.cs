using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class NullSubstituteReferenceException : NotASubstituteException 
    {
        public NullSubstituteReferenceException() { }
#if !SILVERLIGHT
        protected NullSubstituteReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}