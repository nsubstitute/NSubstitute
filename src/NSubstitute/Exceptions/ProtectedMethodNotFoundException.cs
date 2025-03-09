namespace NSubstitute.Exceptions;

public class ProtectedMethodNotFoundException(string message, Exception? innerException) : SubstituteException(message, innerException)
{
    public ProtectedMethodNotFoundException() : this("", null)
    { }

    public ProtectedMethodNotFoundException(string message) : this(message, null)
    { }
}