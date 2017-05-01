using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class ArgumentSetWithIncompatibleValueException : SubstituteException
    {
        const string WhatProbablyWentWrong = "Could not set value of type {2} to argument {0} ({1}) because the types are incompatible.";
        public ArgumentSetWithIncompatibleValueException(int argumentIndex, Type argumentType, Type typeOfValueWeTriedToAssign) 
            : base(string.Format(WhatProbablyWentWrong, argumentIndex, argumentType.Name, typeOfValueWeTriedToAssign.Name)) { }
#if NET35 || NET4 || NET45
        protected ArgumentSetWithIncompatibleValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
