using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Core;
using NSubstitute.Exceptions;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute
{
    public static partial class SubstituteExtensions
    {
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

#nullable restore
        private static void ReThrowOnNSubstituteFault<T>(ValueTask<T?> task)
        {
            if (task.IsFaulted && task.AsTask().Exception!.InnerExceptions.FirstOrDefault() is SubstituteException)
            {
                task.GetAwaiter().GetResult();
            }
        }

        private static ValueTask<T?> CompletedValueTask<T>(T? result) => new(result);

        private static Func<CallInfo, ValueTask<T?>> WrapFuncInValueTask<T>(Func<CallInfo, T> returnThis) =>
            x => CompletedValueTask(returnThis(x));
    }
}