namespace NSubstitute.Exceptions
{
    public class CouldNotConfigureBaseMethodException : SubstituteException
    {
        private const string ExceptionMessage =
            "Cannot configure the base method call as base method implementation is missing. " +
            "You can call base method only if you create a class substitute and the method is not abstract.";

        public CouldNotConfigureBaseMethodException() : base(ExceptionMessage)
        {
        }
    }
}