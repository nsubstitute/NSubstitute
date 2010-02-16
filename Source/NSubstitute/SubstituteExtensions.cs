namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Return<T>(this T value, T returnThis, params T[] returnThese)
        {
            SubstitutionContext.Current.LastInvocationShouldReturn(returnThis);
        }

        public static T Received<T>(this T substitute)
        {
            var handler = SubstitutionContext.Current.GetInvocationHandlerFor(substitute);            
            handler.AssertNextCallHasBeenReceived();
            return substitute;
        }
    }
}