namespace NSubstitute.Exceptions
{
    public class AmbiguousParametersException : SubstituteException
    {
        public AmbiguousParametersException(string message)
            : base(message)
        {
        }
    }
}