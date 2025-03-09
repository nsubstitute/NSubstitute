namespace NSubstitute.Exceptions;

public sealed class CallSequenceNotFoundException(string message) : SubstituteException(message)
{
}
