namespace NSubstitute.Exceptions;

public sealed class ArgumentNotFoundException(string message) : SubstituteException(message)
{
}
