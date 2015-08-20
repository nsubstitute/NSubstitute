#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class MissingSequenceNumberException : SubstituteException
    {
        public MissingSequenceNumberException() { }
#if !DNXCORE50
        protected MissingSequenceNumberException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}