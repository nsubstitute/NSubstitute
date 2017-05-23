using System;
#if NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET45
    [Serializable]
#endif
    public class ArgumentNotFoundException : SubstituteException
    {
        public ArgumentNotFoundException(string message) : base(message) { }
#if NET45
        protected ArgumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
