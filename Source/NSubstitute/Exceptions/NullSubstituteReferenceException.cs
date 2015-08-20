#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class NullSubstituteReferenceException : NotASubstituteException 
    {
        public NullSubstituteReferenceException() { }
#if !DNXCORE50
        protected NullSubstituteReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}