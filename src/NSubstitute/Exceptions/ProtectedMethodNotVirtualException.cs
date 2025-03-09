namespace NSubstitute.Exceptions;

public class ProtectedMethodNotVirtualException(string message, Exception? innerException) : SubstituteException(message, innerException)
{
    public ProtectedMethodNotVirtualException() : this("", null)
    { }

    public ProtectedMethodNotVirtualException(string message) : this(message, null)
    { }
}