using System;

namespace NSubstitute.Exceptions
{
    public class ArgumentSetWithIncompatibleValueException : SubstituteException
    {
        private const string WhatProbablyWentWrong =
            "Could not set value of type {2} to argument {0} ({1}) because the types are incompatible.";

        public ArgumentSetWithIncompatibleValueException(int argumentIndex, Type argumentType, Type typeOfValueWeTriedToAssign)
            : base(string.Format(WhatProbablyWentWrong, argumentIndex, argumentType.Name, typeOfValueWeTriedToAssign.Name)) { }
    }
}
