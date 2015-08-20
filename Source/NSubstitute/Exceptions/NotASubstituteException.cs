#if !DNXCORE50
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
    public class NotASubstituteException : SubstituteException
    {
        const string Explanation = "NSubstitute extension methods like .Received() can only be called on objects created using Substitute.For<T>() and related methods."; 
        public NotASubstituteException() : base(Explanation) { }
#if !DNXCORE50
        protected NotASubstituteException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}