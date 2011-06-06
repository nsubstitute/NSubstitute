using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CouldNotSetReturnException : SubstituteException
    {
        const string WhatProbablyWentWrong = 
                "Could not find a call to return from.\n"+
                "Make sure you called Returns() after calling your substitute (for example: mySub.SomeMethod().Returns(value)).\n" +
                "If you substituted for a class rather than an interface, check that the call to your substitute was on a virtual/abstract member.\n" +
                "Return values cannot be configured for non-virtual/non-abstract members.";

        public CouldNotSetReturnException() : base(WhatProbablyWentWrong) { }
#if !SILVERLIGHT
        protected CouldNotSetReturnException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}