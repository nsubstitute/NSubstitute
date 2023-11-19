using System;
using System.Threading.Tasks;
using NSubstitute.Core;

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
        public static WhenCalled<T> When<T>(this T substitute, Func<T, Task> substituteCall) where T : class
        {
            return MakeWhenCalled(substitute, x => substituteCall(x), MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Perform an action when this member is called with any arguments.
        /// Must be followed by <see cref="WhenCalled{TSubstitute}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<T> WhenForAnyArgs<T>(this T substitute, Func<T, Task> substituteCall) where T : class
        {
            return MakeWhenCalled(substitute, x => substituteCall(x), MatchArgs.Any);
        }
    }
}