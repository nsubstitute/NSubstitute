namespace NSubstitute.Exceptions;

public class ArgumentIsNotOutOrRefException(int argumentIndex, Type argumentType) : SubstituteException(string.Format(WhatProbablyWentWrong, argumentIndex, argumentType.Name))
{
    private const string WhatProbablyWentWrong = "Could not set argument {0} ({1}) as it is not an out or ref argument.";
}
