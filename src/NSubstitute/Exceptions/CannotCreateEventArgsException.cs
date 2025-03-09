namespace NSubstitute.Exceptions;

public sealed class CannotCreateEventArgsException : SubstituteException
{
    public CannotCreateEventArgsException() { }
    public CannotCreateEventArgsException(string message) : base(message) { }
    public CannotCreateEventArgsException(string message, Exception innerException) : base(message, innerException) { }
}
