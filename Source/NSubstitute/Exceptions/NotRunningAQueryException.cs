#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class NotRunningAQueryException : SubstituteException
    {
        public NotRunningAQueryException() { }
#if !DNXCORE50
        protected NotRunningAQueryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}