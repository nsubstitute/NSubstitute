using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class ArgumentNotFoundException : SubstituteException
    {
        public ArgumentNotFoundException(string message) : base(message) { }
#if !SILVERLIGHT
        protected ArgumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}