// Disable nullability for client API, so it does not affect clients.

using System.Collections.Generic;
using NSubstitute.ClearExtensions;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.ReceivedExtensions;

#nullable disable annotations

namespace NSubstitute
{
    public static partial class SubstituteExtensions
    {
        /// <summary>
        /// Checks this substitute has received the following call.
        /// </summary>
        public static T Received<T>(this T substitute) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            return substitute.Received(Quantity.AtLeastOne());
        }

        /// <summary>
        /// Checks this substitute has received the following call the required number of times.
        /// </summary>
        public static T Received<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            return substitute.Received(Quantity.Exactly(requiredNumberOfCalls));
        }

        /// <summary>
        /// Checks this substitute has not received the following call.
        /// </summary>
        public static T DidNotReceive<T>(this T substitute) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            return substitute.Received(Quantity.None());
        }

        /// <summary>
        /// Checks this substitute has received the following call with any arguments.
        /// </summary>
        public static T ReceivedWithAnyArgs<T>(this T substitute) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            return substitute.ReceivedWithAnyArgs(Quantity.AtLeastOne());
        }

        /// <summary>
        /// Checks this substitute has received the following call with any arguments the required number of times.
        /// </summary>
        public static T ReceivedWithAnyArgs<T>(this T substitute, int requiredNumberOfCalls) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            return substitute.ReceivedWithAnyArgs(Quantity.Exactly(requiredNumberOfCalls));
        }

        /// <summary>
        /// Checks this substitute has not received the following call with any arguments.
        /// </summary>
        public static T DidNotReceiveWithAnyArgs<T>(this T substitute) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            return substitute.ReceivedWithAnyArgs(Quantity.None());
        }

        /// <summary>
        /// Returns the calls received by this substitute.
        /// </summary>
        public static IEnumerable<ICall> ReceivedCalls<T>(this T substitute) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            return SubstitutionContext
                .Current
                .GetCallRouterFor(substitute!)
                .ReceivedCalls();
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
            if (substitute == null) throw new NullSubstituteReferenceException();

            substitute.ClearSubstitute(ClearOptions.ReceivedCalls);
        }
    }
}