using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class SequenceInOrderAssertion
    {
        private readonly CallFormatter _callFormatter;

        public SequenceInOrderAssertion()
        {
            _callFormatter = new CallFormatter();
        }

        public void Assert(IQueryResults queryResult)
        {
            var matchingCallsInOrder = queryResult.MatchingCallsInOrder().ToArray();
            var querySpec = queryResult.QuerySpecification().ToArray();

            if (matchingCallsInOrder.Length != querySpec.Length)
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(querySpec, matchingCallsInOrder));
            }

            var callsAndSpecs = matchingCallsInOrder
                .Zip(querySpec, (call, specAndTarget) =>
                                new
                                    {
                                        Call = call,
                                        Spec = specAndTarget.CallSpecification,
                                        IsMatch = Matches(call, specAndTarget)
                                    }
                );

            if (callsAndSpecs.Any(x => !x.IsMatch))
            {
                throw new CallSequenceNotFoundException(GetExceptionMessage(querySpec, matchingCallsInOrder));
            }
        }

        private string GetExceptionMessage(CallSpecAndTarget[] querySpec, ICall[] matchingCallsInOrder)
        {
            const string callDelimiter = "\n    ";
            var formattedQuery = string.Join(callDelimiter,
                                             querySpec.Select(x => x.CallSpecification.ToString()).ToArray());
            var formattedCalls = string.Join(callDelimiter,
                                             matchingCallsInOrder.Select(x => _callFormatter.Format(x.GetMethodInfo(), new [] {""})).ToArray());
            return string.Format("\nExpected to receive these calls in order:{0}{1}\n" + 
                                 "Actually received matching calls in this order:{0}{2}\n\n{3}",
                                 callDelimiter,
                                 formattedQuery,
                                 formattedCalls,
                                 "*** Note: calls to property getters are not considered part of the query. ***");
        }

        private bool Matches(ICall call, CallSpecAndTarget specAndTarget)
        {
            return ReferenceEquals(call.Target(), specAndTarget.Target)
                   && specAndTarget.CallSpecification.IsSatisfiedBy(call);
        }
    }
}