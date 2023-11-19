using System;
using NSubstitute.Core;
using System.Threading.Tasks;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute
{
    public static partial class SubstituteExtensions
    {
        /// <summary>
        /// Perform an action when this member is called.
        /// Must be followed by <see cref="WhenCalled{TSubstitute}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<TSubstitute> When<TSubstitute, TResult>(this TSubstitute substitute,
            Func<TSubstitute, ValueTask<TResult>> substituteCall) where TSubstitute : class
        {
            return MakeWhenCalled(substitute, x => substituteCall(x), MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Perform an action when this member is called with any arguments.
        /// Must be followed by <see cref="WhenCalled{TSubstitute}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<TSubstitute> WhenForAnyArgs<TSubstitute, TResult>(this TSubstitute substitute,
            Func<TSubstitute, ValueTask<TResult>> substituteCall) where TSubstitute : class
        {
            return MakeWhenCalled(substitute, x => substituteCall(x), MatchArgs.Any);
        }
    }
}