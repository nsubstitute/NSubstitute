#if NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class NotRunningAQueryException : SubstituteException
    {
        public NotRunningAQueryException() { }
#if NET4 || NET45
        protected NotRunningAQueryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}