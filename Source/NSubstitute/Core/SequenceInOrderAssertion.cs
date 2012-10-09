using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class SequenceInOrderAssertion
    {
        public void Assert(IQueryResults queryResult)
        {
            var matchingCallsInOrder = queryResult.MatchingCallsInOrder().ToArray();
            var querySpec = queryResult.QuerySpecification().ToArray();

            if (matchingCallsInOrder.Length != querySpec.Length)
            {
                throw new CallSequenceNotFoundException();
            }

            var callsAndSpecs = matchingCallsInOrder
                .Zip(querySpec, (call, specAndTarget) =>
                        new { Call = call, Spec = specAndTarget.CallSpecification, IsMatch = Matches(call, specAndTarget) }
                );

            if (callsAndSpecs.Any(x => !x.IsMatch))
            {
                throw new CallSequenceNotFoundException();
            }
        }

        private bool Matches(ICall call, CallSpecAndTarget specAndTarget)
        {
            return ReferenceEquals(call.Target(), specAndTarget.Target)
                   && specAndTarget.CallSpecification.IsSatisfiedBy(call);
        }
    }
}