using System;
﻿#if NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET45
    [Serializable]
#endif
    public class NotRunningAQueryException : SubstituteException
    {
        public NotRunningAQueryException() { }
#if NET45
        protected NotRunningAQueryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
