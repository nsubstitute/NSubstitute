using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    [Serializable]
    public class ArgumentNotFoundException : SubstituteException
    {
        public ArgumentNotFoundException(string message) : base(message) { }
#if NET35 || NET4 || NET45
        protected ArgumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
