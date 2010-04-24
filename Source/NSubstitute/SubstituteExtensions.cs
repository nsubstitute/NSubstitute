using System;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Returns<T>(this T value, T returnThis, params T[] returnThese)
        {
            var context = SubstitutionContext.Current;
            context.LastCallShouldReturn(returnThis);
        }

        public static T Received<T>(this T substitute)
        {
            var context = SubstitutionContext.Current;
            var router = context.GetCallRouterFor(substitute);            
            router.AssertNextCallHasBeenReceived();
            return substitute;
        }
        
        public static WhenCalled<T> When<T>(this T substitute, Action<T> substituteCall)
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall);            
        }
    }
}