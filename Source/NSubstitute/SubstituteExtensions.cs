using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Routing.Definitions;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Returns<T>(this T value, T returnThis, params T[] returnThese)
        {
            Returns(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
        }

        public static void Returns<T>(this T value, Func<CallInfo,T> returnThis)
        {
            Returns(MatchArgs.AsSpecifiedInCall, returnThis);
        }

        public static void ReturnsForAnyArgs<T>(this T value, T returnThis, params T[] returnThese)
        {
            Returns(MatchArgs.Any, returnThis, returnThese);
        }

        public static void ReturnsForAnyArgs<T>(this T value, Func<CallInfo, T> returnThis)
        {
            Returns(MatchArgs.Any, returnThis);
        }

        private static void Returns<T>(MatchArgs matchArgs, T returnThis, params T[] returnThese)
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
            context.LastCallShouldReturn(returnValue, matchArgs);
        }

        private static void Returns<T>(MatchArgs matchArgs, Func<CallInfo,T> returnThis)
        {
            var context = SubstitutionContext.Current;
            var returnValue = new ReturnValueFromFunc<T>(returnThis);
            context.LastCallShouldReturn(returnValue, matchArgs);
        }

        public static T Received<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallReceivedRoute>(MatchArgs.AsSpecifiedInCall);
            return substitute;
        }

        public static T Received<T>(this T substitute, int expectedCount) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallReceivedExactlyRoute>(MatchArgs.AsSpecifiedInCall, expectedCount);
            return substitute;
        }

        public static T DidNotReceive<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallNotReceivedRoute>(MatchArgs.AsSpecifiedInCall);
            return substitute;
        }

        public static T ReceivedWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallReceivedRoute>(MatchArgs.Any);
            return substitute;
        }

        public static T ReceivedWithAnyArgs<T>(this T substitute, int expectedCount) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallReceivedExactlyRoute>(MatchArgs.Any, expectedCount);
            return substitute;
        }
        
        public static T DidNotReceiveWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute<CheckCallNotReceivedRoute>(MatchArgs.Any);
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
            return new WhenCalled<T>(context, substitute, substituteCall, MatchArgs.AsSpecifiedInCall);            
        }

        public static WhenCalled<T> WhenForAnyArgs<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall, MatchArgs.Any);            
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