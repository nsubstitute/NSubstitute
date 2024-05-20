namespace NSubstitute.Exceptions;

public class SubstituteException(string message, Exception? innerException) : Exception(message, innerException)
{
    public SubstituteException() : this("") { }
    public SubstituteException(string message) : this(message, null) { }
}
