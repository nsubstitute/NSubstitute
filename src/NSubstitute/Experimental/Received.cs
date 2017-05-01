using System;
using NSubstitute.Core;
using NSubstitute.Core.SequenceChecking;

namespace NSubstitute.Experimental
{
    [Obsolete("Use NSubstitute.Received instead of NSubstitute.Experimental.Received")]
    public class Received
    {
        /// <summary>
        /// *EXPERIMENTAL* Asserts the calls to the substitutes contained in the given Action were
        /// received by these substitutes in the same order. Calls to property getters are not included
        /// in the assertion.
        /// </summary>
        /// <param name="calls">Action containing calls to substitutes in the expected order</param>
        [Obsolete("Use NSubstitute.Received.InOrder instead of NSubstitute.Experimental.Received.InOrder")]
        public static void InOrder(Action calls)
        {
            var queryResult = SubstitutionContext.Current.RunQuery(calls);
            new SequenceInOrderAssertion().Assert(queryResult);
        }
    }
}