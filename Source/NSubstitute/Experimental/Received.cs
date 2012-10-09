using System;
using NSubstitute.Core;

namespace NSubstitute.Experimental
{
    public class Received
    {
        public static void InOrder(Action calls)
        {
            var queryResult = SubstitutionContext.Current.RunQuery(calls);
            new SequenceInOrderAssertion().Assert(queryResult);
        }
    }
}