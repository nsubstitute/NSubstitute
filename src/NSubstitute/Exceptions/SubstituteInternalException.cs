namespace NSubstitute.Exceptions;


public class SubstituteInternalException(string message, Exception? innerException) : SubstituteException("Please report this exception at https://github.com/nsubstitute/NSubstitute/issues: \n\n" + message,
        innerException)
{
    public SubstituteInternalException() : this("") { }
    public SubstituteInternalException(string message) : this(message, null) { }
}