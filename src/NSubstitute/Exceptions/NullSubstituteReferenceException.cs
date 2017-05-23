using System;
#if NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET45
    [Serializable]
#endif
    public class NullSubstituteReferenceException : NotASubstituteException
    {
        public NullSubstituteReferenceException() { }
#if NET45
        protected NullSubstituteReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
