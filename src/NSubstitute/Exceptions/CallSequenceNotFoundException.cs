using System;
﻿#if NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET45
    [Serializable]
#endif
    public class CallSequenceNotFoundException : SubstituteException
    {
        public CallSequenceNotFoundException(string message) : base(message) { }
#if NET45
        protected CallSequenceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
