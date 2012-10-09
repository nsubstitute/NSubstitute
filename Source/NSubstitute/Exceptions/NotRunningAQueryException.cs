using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class NotRunningAQueryException : SubstituteException
    {
        public NotRunningAQueryException() { }
#if !SILVERLIGHT
        protected NotRunningAQueryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}