namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Return<T>(this T valueBeingExtended, T returnThis)
        {
            SubstitutionContext.Current.LastCallShouldReturn(returnThis);
        }
    }
}