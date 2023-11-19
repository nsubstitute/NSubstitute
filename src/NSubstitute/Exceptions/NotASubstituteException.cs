namespace NSubstitute.Exceptions
{
    public class NotASubstituteException : SubstituteException
    {
        private const string Explanation =
            "NSubstitute extension methods like .Received() can only be called " +
            "on objects created using Substitute.For<T>() and related methods.";

        public NotASubstituteException() : base(Explanation) { }
    }
}
