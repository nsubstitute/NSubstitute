namespace NSubstitute.Exceptions;

public sealed class ProtectedMethodNotFoundException(string message, Exception? innerException) : SubstituteException(message, innerException)
{
    public ProtectedMethodNotFoundException() : this("", null)
    { }

    public ProtectedMethodNotFoundException(string message) : this(message, null)
    { }
}