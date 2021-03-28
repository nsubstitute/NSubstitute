namespace NSubstitute.Exceptions
{
    public class NoBaseImplementationException : SubstituteException
    {
        private const string Explanation =
            "Cannot call the base method as the base method implementation is missing. " +
            "You can call base method only if you create a class substitute and the method is not abstract.";

        public NoBaseImplementationException() : base(Explanation) { }
    }
}
