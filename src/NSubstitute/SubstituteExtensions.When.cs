using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute
{
    public static partial class SubstituteExtensions
    {
        /// <summary>
        /// Perform an action when this member is called.
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<T> When<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            return MakeWhenCalled(substitute, substituteCall, MatchArgs.AsSpecifiedInCall);
        }

        /// <summary>
        /// Perform an action when this member is called with any arguments.
        /// Must be followed by <see cref="WhenCalled{T}.Do(Action{CallInfo})"/> to provide the callback.
        /// </summary>
        public static WhenCalled<T> WhenForAnyArgs<T>(this T substitute, Action<T> substituteCall) where T : class
        {
            return MakeWhenCalled(substitute, substituteCall, MatchArgs.Any);
        }

#nullable restore
        private static WhenCalled<TSubstitute> MakeWhenCalled<TSubstitute>(TSubstitute? substitute,
            Action<TSubstitute> action, MatchArgs matchArgs)
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            var context = SubstitutionContext.Current;
            return new WhenCalled<TSubstitute>(context, substitute, action, matchArgs);
        }
    }
}