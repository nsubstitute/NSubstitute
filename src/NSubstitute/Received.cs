﻿using System;
using NSubstitute.Core;
using NSubstitute.Core.SequenceChecking;

namespace NSubstitute
{
    public class Received
    {
        /// <summary>
        /// Asserts the calls to the substitutes contained in the given Action were
        /// received by these substitutes in the same order. Calls to property getters are not included
        /// in the assertion.
        /// </summary>
        /// <param name="calls">Action containing calls to substitutes in the expected order</param>
        public static void InOrder(Action calls)
        {
            var query = new Query();
            SubstitutionContext.Current.ThreadContext.RunInQueryContext(calls, query);
            new SequenceInOrderAssertion().Assert(query.Result());
        }
    }
}