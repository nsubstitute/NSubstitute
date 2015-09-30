using System;
using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    [Serializable]
    public class NotASubstituteException : SubstituteException
    {
        const string Explanation = "NSubstitute extension methods like .Received() can only be called on objects created using Substitute.For<T>() and related methods."; 
        public NotASubstituteException() : base(Explanation) { }
        protected NotASubstituteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
