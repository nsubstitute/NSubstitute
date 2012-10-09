using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CallSequenceNotFoundException : SubstituteException
    {
        public CallSequenceNotFoundException() { }

#if !SILVERLIGHT
        protected CallSequenceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}