using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class UnexpectedArgumentMatcherException : SubstituteException
    {
        public static string WhatProbablyWentWrong =
            "Argument matchers (Arg.Is, Arg.Any) should only be used in place of member arguments. " +
            "Do not use in a Returns() statement or anywhere else outside of a member call." + Environment.NewLine +
            "Correct use:" + Environment.NewLine +
            "  sub.MyMethod(Arg.Any<string>()).Returns(\"hi\")" + Environment.NewLine +
            "Incorrect use:" + Environment.NewLine +
            "  sub.MyMethod(\"hi\").Returns(Arg.Any<string>())";
        public UnexpectedArgumentMatcherException() : this(WhatProbablyWentWrong) { }
        public UnexpectedArgumentMatcherException(string message) : base(message) { }
#if NET35 || NET4 || NET45
        protected UnexpectedArgumentMatcherException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
