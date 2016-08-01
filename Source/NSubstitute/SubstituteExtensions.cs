using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.ClearExtensions;
#if (NET4 || NET45 || NETSTANDARD1_5)
using System.Threading.Tasks;
#endif

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        /// <summary>
        /// Set a return value for this call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        /// <returns></returns>
        public static ConfiguredCall Returns<T>(this T value, T returnThis, params T[] returnThese)
        {
            return Returns(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
        }

        /// <summary>
        /// Set a return value for this call, calculated by the provided function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        /// <returns></returns>
        public static ConfiguredCall Returns<T>(this T value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            return Returns(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
        }

#if (NET4 || NET45 || NETSTANDARD1_5)
        /// <summary>
        /// Set a return value for this call. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return. Will be wrapped in a Task</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        /// <returns></returns>
        public static ConfiguredCall Returns<T>(this Task<T> value, T returnThis, params T[] returnThese)
        {
            var wrappedReturnValue = CompletedTask(returnThis);

            var wrappedParameters = returnThese.Select(CompletedTask);

            return Returns(MatchArgs.AsSpecifiedInCall, wrappedReturnValue, wrappedParameters.ToArray());
        }

        /// <summary>
        /// Set a return value for this call, calculated by the provided function. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        /// <returns></returns>
        public static ConfiguredCall Returns<T>(this Task<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            var wrappedFunc = WrapFuncInTask(returnThis);
            var wrappedFuncs = returnThese.Select(WrapFuncInTask);

            return Returns(MatchArgs.AsSpecifiedInCall, wrappedFunc, wrappedFuncs.ToArray());
        }

        /// <summary>
        /// Set a return value for this call made with any arguments. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this Task<T> value, T returnThis, params T[] returnThese)
        {
            var wrappedReturnValue = CompletedTask(returnThis);

            var wrappedParameters = returnThese.Select(CompletedTask);

            return Returns(MatchArgs.Any, wrappedReturnValue, wrappedParameters.ToArray());
        }

        /// <summary>
        /// Set a return value for this call made with any arguments, calculated by the provided function. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this Task<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            var wrappedFunc = WrapFuncInTask(returnThis);
            var wrappedFuncs = returnThese.Select(WrapFuncInTask);
            
            return Returns(MatchArgs.Any, wrappedFunc, wrappedFuncs.ToArray());
        }
#endif

        /// <summary>
        /// Set a return value for this call made with any arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this T value, T returnThis, params T[] returnThese)
        {
            return Returns(MatchArgs.Any, returnThis, returnThese);
        }

        /// <summary>
        /// Set a return value for this call made with any arguments, calculated by the provided function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this T value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            return Returns(MatchArgs.Any, returnThis, returnThese);
        }

        internal static ConfiguredCall Returns<T>(MatchArgs matchArgs, T returnThis, params T[] returnThese)
        {
            var context = SubstitutionContext.Current;
            IReturn returnValue;
            if (returnThese == null || returnThese.Length == 0)
            {
                returnValue = new ReturnValue(returnThis);
            }
            else
            {
                returnValue = new ReturnMultipleValues<T>(new[] { returnThis }.Concat(returnThese));
            }
            return context.LastCallShouldReturn(returnValue, matchArgs);
        }

        internal static ConfiguredCall Returns<T>(MatchArgs matchArgs, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
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

            return context.LastCallShouldReturn(returnValue, matchArgs);
        }

        /// <summary>
        /// Checks this substitute has received the following call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <returns></returns>
        public static T Received<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.AsSpecifiedInCall, Quantity.AtLeastOne()));
            return substitute;
        }

        /// <summary>
        /// Checks this substitute has received the following call the required number of times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="requiredNumberOfCalls"></param>
        /// <returns></returns>
        public static T Received<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.AsSpecifiedInCall, Quantity.Exactly(requiredNumberOfCalls)));
            return substitute;
        }

        /// <summary>
        /// Checks this substitute has not received the following call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <returns></returns>
        public static T DidNotReceive<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.AsSpecifiedInCall, Quantity.None()));
            return substitute;
        }

        /// <summary>
        /// Checks this substitute has received the following call with any arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <returns></returns>
        public static T ReceivedWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.Any, Quantity.AtLeastOne()));
            return substitute;
        }

        /// <summary>
        /// Checks this substitute has received the following call with any arguments the required number of times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="requiredNumberOfCalls"></param>
        /// <returns></returns>
        public static T ReceivedWithAnyArgs<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.Any, Quantity.Exactly(requiredNumberOfCalls)));
            return substitute;
        }

        /// <summary>
        /// Checks this substitute has not received the following call with any arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <returns></returns>
        public static T DidNotReceiveWithAnyArgs<T>(this T substitute) where T : class
        {
            var router = GetRouterForSubstitute(substitute);
            router.SetRoute(x => RouteFactory().CheckReceivedCalls(x, MatchArgs.Any, Quantity.None()));
            return substitute;
        }

        /// <summary>
        /// Forget all the calls this substitute has received.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <remarks>
        /// Note that this will not clear any results set up for the substitute using Returns().
        /// See <see cref="NSubstitute.ClearExtensions.ClearExtensions.ClearSubstitute{T}"/> for more options with resetting 
        /// a substitute.
        /// </remarks>
        public static void ClearReceivedCalls<T>(this T substitute) where T : class
        {
            substitute.ClearSubstitute(ClearOptions.ReceivedCalls);
        }

        /// <summary>
        /// Perform an action when this member is called. 
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="substituteCall"></param>
        /// <returns></returns>
        public static WhenCalled<T> When<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall, MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Perform an action when this member is called with any arguments. 
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="substituteCall"></param>
        /// <returns></returns>
        public static WhenCalled<T> WhenForAnyArgs<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall, MatchArgs.Any);
        }

        /// <summary>
        /// Returns the calls received by this substitute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <returns></returns>
        public static IEnumerable<ICall> ReceivedCalls<T>(this T substitute) where T : class
        {
            return GetRouterForSubstitute(substitute).ReceivedCalls();
        }

#if (NET4 || NET45 || NETSTANDARD1_5)
        private static Func<CallInfo, Task<T>> WrapFuncInTask<T>(Func<CallInfo, T> returnThis)
        {
            return x => CompletedTask(returnThis(x));
        }

        private static Task<T> CompletedTask<T>(T result) 
        {
#if (NET45 || NETSTANDARD1_5)
            return Task.FromResult(result);
#elif NET4
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
#endif
        }
#endif

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