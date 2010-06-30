using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Routes;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        private const bool MatchArgsSpecifiedInCall = false;
        private const bool MatchAnyArgs = true;

        public static void Returns<T>(this T value, T returnThis, params T[] returnThese)
        {
            Returns(false, returnThis, returnThese);
        }

        public static void Returns<T>(this T value, Func<CallInfo,T> returnThis)
        {
            Returns(false, returnThis);
        }

        public static void ReturnsForAnyArgs<T>(this T value, T returnThis, params T[] returnThese)
        {
            Returns(true, returnThis, returnThese);
        }

        public static void ReturnsForAnyArgs<T>(this T value, Func<CallInfo, T> returnThis)
        {
            Returns(true, returnThis);
        }

        private static void Returns<T>(bool ignoreCallArgs, T returnThis, params T[] returnThese)
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
            context.LastCallShouldReturn(returnValue, ignoreCallArgs);
        }

        private static void Returns<T>(bool ignoreCallArgs, Func<CallInfo,T> returnThis)
        {
            var context = SubstitutionContext.Current;
            var returnValue = new ReturnValueFromFunc<T>(returnThis);
            context.LastCallShouldReturn(returnValue, ignoreCallArgs);
        }

        public static T Received<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallReceivedRoute>(MatchArgsSpecifiedInCall);
            return substitute;
        }

        public static T DidNotReceive<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallNotReceivedRoute>(MatchArgsSpecifiedInCall);
            return substitute;
        }

        public static T ReceivedWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallReceivedRoute>(MatchAnyArgs);
            return substitute;
        }
        
        public static T DidNotReceiveWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallNotReceivedRoute>(MatchAnyArgs);
            return substitute;
        }

        public static void ClearReceivedCalls<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.ClearReceivedCalls();
        }

        public static WhenCalled<T> When<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall);            
        }

        public static IEnumerable<ICall> ReceivedCalls<T>(this T substitute) where T : class
        {
            return GetRouterForSubstitute(substitute).ReceivedCalls();
        }

        private static ICallRouter GetRouterForSubstitute<T>(T substitute)
        {
            var context = SubstitutionContext.Current;
            return context.GetCallRouterFor(substitute);
        }
    }
}