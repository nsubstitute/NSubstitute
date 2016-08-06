using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class ArgumentIsNotOutOrRefException : SubstituteException
    {
        const string WhatProbablyWentWrong = "Could not set argument {0} ({1}) as it is not an out or ref argument.";
        public ArgumentIsNotOutOrRefException(int argumentIndex, Type argumentType) : base(string.Format(WhatProbablyWentWrong, argumentIndex, argumentType.Name)) { }
#if NET35 || NET4 || NET45
        protected ArgumentIsNotOutOrRefException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
