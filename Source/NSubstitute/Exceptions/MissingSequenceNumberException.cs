using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class MissingSequenceNumberException : SubstituteException
    {
        public MissingSequenceNumberException() { }
        protected MissingSequenceNumberException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}