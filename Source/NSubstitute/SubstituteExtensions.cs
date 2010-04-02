using System;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Return<T>(this T value, T returnThis, params T[] returnThese)
        {
            var context = SubstitutionContext.Current;
            context.LastCallShouldReturn(returnThis);
        }

        public static T Received<T>(this T substitute)
        {
            var context = SubstitutionContext.Current;
            var handler = context.GetCallHandlerFor(substitute);            
            handler.AssertNextCallHasBeenReceived();
            return substitute;
        }

        public static void Raise<T>(this T substitute, Action<T> eventReference, params object[] eventArguments)
        {
            var context = SubstitutionContext.Current;
            var handler = context.GetCallHandlerFor(substitute);
            handler.RaiseEventFromNextCall(eventArguments);
            eventReference(substitute);
        }
    }
}