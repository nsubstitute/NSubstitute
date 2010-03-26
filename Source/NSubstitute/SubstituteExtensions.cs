using System;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Return<T>(this T value, T returnThis, params T[] returnThese)
        {
            ISubstitutionContext context = SubstitutionContext.Current;
            context.LastCallShouldReturn(returnThis, context.RetrieveArgumentMatchers());
        }

        public static T Received<T>(this T substitute)
        {
            ISubstitutionContext context = SubstitutionContext.Current;
            var handler = context.GetCallHandlerFor(substitute);            
            handler.AssertNextCallHasBeenReceived();
            return substitute;
        }

        public static void Raise<T>(this T substitute, Action<T> eventReference, params object[] eventArguments)
        {
        }
    }
}