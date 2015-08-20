#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class ArgumentNotFoundException : SubstituteException
    {
        public ArgumentNotFoundException(string message) : base(message) { }
#if !DNXCORE50
        protected ArgumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}