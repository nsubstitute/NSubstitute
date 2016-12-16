using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{

#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class SubstituteInternalException : SubstituteException
    {
        public SubstituteInternalException() : this("") { }
        public SubstituteInternalException(string message) : this(message, null) { }
        public SubstituteInternalException(string message, Exception innerException)
            : base("Please report this exception at https://github.com/nsubstitute/NSubstitute/issues: \n\n" + message,
                innerException) { }
#if NET35 || NET4 || NET45
        protected SubstituteInternalException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}