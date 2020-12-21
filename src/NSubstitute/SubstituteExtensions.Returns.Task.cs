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

#nullable restore
        private static void ReThrowOnNSubstituteFault<T>(Task<T?> task)
        {
            if (task.IsFaulted && task.Exception!.InnerExceptions.FirstOrDefault() is SubstituteException)
            {
                task.GetAwaiter().GetResult();
            }
        }

        private static Task<T?> CompletedTask<T>(T? result) => Task.FromResult(result);

        private static Func<CallInfo, Task<T?>> WrapFuncInTask<T>(Func<CallInfo, T> returnThis) =>
            x => CompletedTask(returnThis(x));
    }
}