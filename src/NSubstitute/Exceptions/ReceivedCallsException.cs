namespace NSubstitute.Exceptions;

public sealed class ReceivedCallsException : SubstituteException
{
    public ReceivedCallsException() { }
    public ReceivedCallsException(string message) : base(message) { }
    public ReceivedCallsException(string message, Exception innerException) : base(message, innerException) { }
}
