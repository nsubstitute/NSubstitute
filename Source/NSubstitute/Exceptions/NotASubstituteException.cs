using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class NotASubstituteException : SubstituteException
    {
        const string Explanation = "NSubstitute extension methods like .Received() can only be called on objects created using Substitute.For<T>() and related methods.";
        public NotASubstituteException() : base(Explanation) { }
#if NET35 || NET4 || NET45
        protected NotASubstituteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
