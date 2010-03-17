namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Return<T>(this T value, T returnThis, params T[] returnThese)
        {
            SubstitutionContext.Current.LastCallShouldReturn(returnThis);
        }

        public static T Received<T>(this T substitute)
        {
            var handler = SubstitutionContext.Current.GetCallHandlerFor(substitute);            
            handler.AssertNextCallHasBeenReceived();
            return substitute;
        }
    }
}