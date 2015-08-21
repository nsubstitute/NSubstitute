#if NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class NullSubstituteReferenceException : NotASubstituteException 
    {
        public NullSubstituteReferenceException() { }
#if NET4 || NET45
        protected NullSubstituteReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}