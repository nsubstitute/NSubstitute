using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.ClearExtensions;
using System.Threading.Tasks;
using NSubstitute.Exceptions;
using NSubstitute.ReceivedExtensions;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        /// <summary>
        /// Set a return value for this call.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        public static ConfiguredCall Returns<T>(this T value, T returnThis, params T[] returnThese)
        {
            return ConfigureReturn(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
        }

        /// <summary>
        /// Set a return value for this call, calculated by the provided function.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        public static ConfiguredCall Returns<T>(this T value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            return ConfigureReturn(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);
        }

        /// <summary>
        /// Set a return value for this call. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return. Will be wrapped in a Task</param>
        /// <param name="returnThese">Optionally use these values next</param>
        public static ConfiguredCall Returns<T>(this Task<T> value, T returnThis, params T[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);
 
            var wrappedReturnValue = CompletedTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(CompletedTask).ToArray() : null;

            return ConfigureReturn(MatchArgs.AsSpecifiedInCall, wrappedReturnValue, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call, calculated by the provided function. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        public static ConfiguredCall Returns<T>(this Task<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);
 
            var wrappedFunc = WrapFuncInTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(WrapFuncInTask).ToArray() : null;

            return ConfigureReturn(MatchArgs.AsSpecifiedInCall, wrappedFunc, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call. The value(s) to be returned will be wrapped in ValueTasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return. Will be wrapped in a ValueTask</param>
        /// <param name="returnThese">Optionally use these values next</param>
        public static ConfiguredCall Returns<T>(this ValueTask<T> value, T returnThis, params T[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);
 
            var wrappedReturnValue = CompletedValueTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(CompletedValueTask).ToArray() : null;

            return ConfigureReturn(MatchArgs.AsSpecifiedInCall, wrappedReturnValue, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call, calculated by the provided function. The value(s) to be returned will be wrapped in ValueTasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        public static ConfiguredCall Returns<T>(this ValueTask<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);
 
            var wrappedFunc = WrapFuncInValueTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(WrapFuncInValueTask).ToArray() : null;

            return ConfigureReturn(MatchArgs.AsSpecifiedInCall, wrappedFunc, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call made with any arguments. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this Task<T> value, T returnThis, params T[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);

            var wrappedReturnValue = CompletedTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(CompletedTask).ToArray() : null;

            return ConfigureReturn(MatchArgs.Any, wrappedReturnValue, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call made with any arguments, calculated by the provided function. The value(s) to be returned will be wrapped in Tasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this Task<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);

            var wrappedFunc = WrapFuncInTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(WrapFuncInTask).ToArray() : null;
            
            return ConfigureReturn(MatchArgs.Any, wrappedFunc, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call made with any arguments. The value(s) to be returned will be wrapped in ValueTasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this ValueTask<T> value, T returnThis, params T[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);

            var wrappedReturnValue = CompletedValueTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(CompletedValueTask).ToArray() : null;

            return ConfigureReturn(MatchArgs.Any, wrappedReturnValue, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call made with any arguments, calculated by the provided function. The value(s) to be returned will be wrapped in ValueTasks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this ValueTask<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
        {
            ReThrowOnNSubstituteFault(value);

            var wrappedFunc = WrapFuncInValueTask(returnThis);
            var wrappedReturnThese = returnThese.Length > 0 ? returnThese.Select(WrapFuncInValueTask).ToArray() : null;

            return ConfigureReturn(MatchArgs.Any, wrappedFunc, wrappedReturnThese);
        }

        /// <summary>
        /// Set a return value for this call made with any arguments.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this T value, T returnThis, params T[] returnThese)
        {
            return ConfigureReturn(MatchArgs.Any, returnThis, returnThese);
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
            return ConfigureReturn(MatchArgs.Any, returnThis, returnThese);
        }

        private static ConfiguredCall ConfigureReturn<T>(MatchArgs matchArgs, T returnThis, T[] returnThese)
        {
            IReturn returnValue;
            if (returnThese == null || returnThese.Length == 0)
            {
                returnValue = new ReturnValue(returnThis);
            }
            else
            {
                returnValue = new ReturnMultipleValues<T>(new[] { returnThis }.Concat(returnThese).ToArray());
            }
            return SubstitutionContext
                .Current
                .ThreadContext
                .LastCallShouldReturn(returnValue, matchArgs);
        }

        private static ConfiguredCall ConfigureReturn<T>(MatchArgs matchArgs, Func<CallInfo, T> returnThis, Func<CallInfo, T>[] returnThese)
        {
            IReturn returnValue;
            if (returnThese == null || returnThese.Length == 0)
            {
                returnValue = new ReturnValueFromFunc<T>(returnThis);
            }
            else
            {
                returnValue = new ReturnMultipleFuncsValues<T>(new[] { returnThis }.Concat(returnThese).ToArray());
            }

            return SubstitutionContext
                .Current
                .ThreadContext
                .LastCallShouldReturn(returnValue, matchArgs);
        }

        /// <summary>
        /// Checks this substitute has received the following call.
        /// </summary>
        public static T Received<T>(this T substitute) where T : class
        {
            return substitute.Received(Quantity.AtLeastOne());
        }

        /// <summary>
        /// Checks this substitute has received the following call the required number of times.
        /// </summary>
        public static T Received<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            return substitute.Received(Quantity.Exactly(requiredNumberOfCalls));
        }

        /// <summary>
        /// Checks this substitute has not received the following call.
        /// </summary>
        public static T DidNotReceive<T>(this T substitute) where T : class
        {
            return substitute.Received(Quantity.None());
        }

        /// <summary>
        /// Checks this substitute has received the following call with any arguments.
        /// </summary>
        public static T ReceivedWithAnyArgs<T>(this T substitute) where T : class
        {
            return substitute.ReceivedWithAnyArgs(Quantity.AtLeastOne());
        }

        /// <summary>
        /// Checks this substitute has received the following call with any arguments the required number of times.
        /// </summary>
        public static T ReceivedWithAnyArgs<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            return substitute.ReceivedWithAnyArgs(Quantity.Exactly(requiredNumberOfCalls));
        }

        /// <summary>
        /// Checks this substitute has not received the following call with any arguments.
        /// </summary>
        public static T DidNotReceiveWithAnyArgs<T>(this T substitute) where T : class
        {
            return substitute.ReceivedWithAnyArgs(Quantity.None());
        }

        /// <summary>
        /// Forget all the calls this substitute has received.
        /// </summary>
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
        public static WhenCalled<T> When<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall, MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Perform an action when this member is called. 
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<T> When<T>(this T substitute, Func<T, Task> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, x => substituteCall(x), MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Perform an action when this member is called. 
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<TSubstitute> When<TSubstitute, TResult>(this TSubstitute substitute, Func<TSubstitute, ValueTask<TResult>> substituteCall) where TSubstitute : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<TSubstitute>(context, substitute, x => substituteCall(x), MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Perform an action when this member is called with any arguments. 
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<T> WhenForAnyArgs<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, substituteCall, MatchArgs.Any);
        }

        /// <summary>
        /// Perform an action when this member is called with any arguments. 
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<T> WhenForAnyArgs<T>(this T substitute, Func<T, Task> substituteCall) where T : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<T>(context, substitute, x => substituteCall(x), MatchArgs.Any);
        }

        /// <summary>
        /// Perform an action when this member is called with any arguments. 
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<TSubstitute> WhenForAnyArgs<TSubstitute, TResult>(this TSubstitute substitute, Func<TSubstitute, ValueTask<TResult>> substituteCall) where TSubstitute : class
        {
            var context = SubstitutionContext.Current;
            return new WhenCalled<TSubstitute>(context, substitute, x => substituteCall(x), MatchArgs.Any);
        }

        /// <summary>
        /// Returns the calls received by this substitute.
        /// </summary>
        public static IEnumerable<ICall> ReceivedCalls<T>(this T substitute) where T : class
        {
            return SubstitutionContext
                .Current
                .GetCallRouterFor(substitute)
                .ReceivedCalls();
        }

        private static Func<CallInfo, Task<T>> WrapFuncInTask<T>(Func<CallInfo, T> returnThis)
        {
            return x => CompletedTask(returnThis(x));
        }

        private static Func<CallInfo, ValueTask<T>> WrapFuncInValueTask<T>(Func<CallInfo, T> returnThis)
        {
            return x => CompletedValueTask(returnThis(x));
        }

        private static Task<T> CompletedTask<T>(T result) 
        {
            return Task.FromResult(result);
        }

        private static ValueTask<T> CompletedValueTask<T>(T result)
        {
            return new ValueTask<T>(result);
        }

        private static void ReThrowOnNSubstituteFault<T>(Task<T> task)
        {
            if (task.IsFaulted && task.Exception.InnerExceptions.FirstOrDefault() is SubstituteException)
            {
                task.GetAwaiter().GetResult();
            }
        }

        private static void ReThrowOnNSubstituteFault<T>(ValueTask<T> task)
        {
            if (task.IsFaulted && task.AsTask().Exception.InnerExceptions.FirstOrDefault() is SubstituteException)
            {
                task.GetAwaiter().GetResult();
            }
        }
    }
}