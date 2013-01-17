using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Routing;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Returns<T>(this T value, T returnThis, params T[] returnThese)
        {
            Returns(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
        }

        public static void Returns<T>(this T value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            Returns(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
        }

        public static void ReturnsForAnyArgs<T>(this T value, T returnThis, params T[] returnThese)
        {
            Returns(MatchArgs.Any, returnThis, returnThese);
        }

        public static void ReturnsForAnyArgs<T>(this T value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            Returns(MatchArgs.Any, returnThis, returnThese);
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

        private static void Returns<T>(MatchArgs matchArgs, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            var context = SubstitutionContext.Current;
            IReturn returnValue;
            if (returnThese == null || returnThese.Length == 0)
            {
                returnValue = new ReturnValueFromFunc<T>(returnThis);
            }
            else
            {
                returnValue = new ReturnMultipleFuncsValues<T>(new[] { returnThis }.Concat(returnThese));
            }

            context.LastCallShouldReturn(returnValue, matchArgs);
        }

        public static T Received<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.AsSpecifiedInCall, Quantity.AtLeastOne()));
            return substitute;
        }

        public static T Received<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.AsSpecifiedInCall, Quantity.Exactly(requiredNumberOfCalls)));
            return substitute;
        }

        public static T DidNotReceive<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.AsSpecifiedInCall, Quantity.None()));
            return substitute;
        }

        public static T ReceivedWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.Any, Quantity.AtLeastOne()));
            return substitute;
        }

        public static T ReceivedWithAnyArgs<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.Any, Quantity.Exactly(requiredNumberOfCalls)));
            return substitute;
        }
        
        public static T DidNotReceiveWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.Any, Quantity.None()));
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

        private static IRouteFactory RouteFactory()
        {
            return SubstitutionContext.Current.GetRouteFactory();
        }
    }
}