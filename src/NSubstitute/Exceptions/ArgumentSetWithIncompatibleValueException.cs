namespace NSubstitute.Exceptions;

public class ArgumentSetWithIncompatibleValueException(int argumentIndex, Type argumentType, Type typeOfValueWeTriedToAssign) : SubstituteException(string.Format(WhatProbablyWentWrong, argumentIndex, argumentType.Name, typeOfValueWeTriedToAssign.Name))
{
    private const string WhatProbablyWentWrong =
        "Could not set value of type {2} to argument {0} ({1}) because the types are incompatible.";
}
