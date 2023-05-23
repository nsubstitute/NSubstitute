namespace NSubstitute.Exceptions;

public class DoAnyTypeException : SubstituteException
{
    private const string FixedMessage = "Use DoForAny() instead of Do<AnyType>()";
    public DoAnyTypeException() : base(FixedMessage) { }
}