#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class CallSequenceNotFoundException : SubstituteException
    {
        public CallSequenceNotFoundException(string message) : base(message) { }
#if !DNXCORE50
        protected CallSequenceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}