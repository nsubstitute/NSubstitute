namespace NSubstitute.Exceptions
{
    public class NullSubstituteReferenceException : SubstituteException
    {
        private const string Explanation = "NSubstitute extension methods like .Received() can only be called on non-null objects.";

        public NullSubstituteReferenceException() : base(Explanation) { }
    }
}
