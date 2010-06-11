using System;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Routes;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Returns<T>(this T value, T returnThis, params T[] returnThese)
        {
            var context = SubstitutionContext.Current;
            IReturn returnValue;
            if (returnThese == null || returnThese.Length == 0)
            {
                returnValue = new ReturnValue(returnThis);
            }
            else
            {            
                returnValue = new ReturnMultipleValues<T>(new[] {returnThis}.Concat(returnThese));
            }
            context.LastCallShouldReturn(returnValue);
        }

        public static void Returns<T>(this T value, Func<CallInfo,T> returnThis)
        {
            var context = SubstitutionContext.Current;
            var returnValue = new ReturnValueFromFunc<T>(returnThis);
            context.LastCallShouldReturn(returnValue);
        }

        public static T Received<T>(this T substitute)
        {
            var context = SubstitutionContext.Current;
            var router = context.GetCallRouterFor(substitute);
            router.SetRoute<CheckCallReceivedRoute>();
            return substitute;
        }
        
        public static WhenCalled<T> When<T>(this T substitute, Action<T> substituteCall)
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall);            
        }

        public static void ClearReceivedCalls<T>(this T substitute)
        {
            var context = SubstitutionContext.Current;
            var router = context.GetCallRouterFor(substitute);
            router.ClearReceivedCalls();
        }
    }
}