using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core.SequenceChecking
{
    public class SequenceInOrderAssertion
    {
        public void Assert(IQueryResults queryResult)
        {
            var matchingCallsInOrder = queryResult
                .MatchingCallsInOrder()
                .Where(x => IsNotPropertyGetterCall(x.GetMethodInfo()))
                .ToArray();
            var querySpec = queryResult
                .QuerySpecification()
                .Where(x => IsNotPropertyGetterCall(x.CallSpecification.GetMethodInfo()))
                .ToArray();

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

        private bool Matches(ICall call, CallSpecAndTarget specAndTarget)
        {
            return ReferenceEquals(call.Target(), specAndTarget.Target)
                   && specAndTarget.CallSpecification.IsSatisfiedBy(call);
        }

        private bool IsNotPropertyGetterCall(MethodInfo methodInfo)
        {
            return methodInfo.GetPropertyFromGetterCallOrNull() == null;
        }

        private string GetExceptionMessage(CallSpecAndTarget[] querySpec, ICall[] matchingCallsInOrder)
        {
            const string callDelimiter = "\n    ";
            var formatter = new SequenceFormatter(callDelimiter, querySpec, matchingCallsInOrder);
            return string.Format("\nExpected to receive these calls in order:\n{0}{1}\n" +
                                 "\nActually received matching calls in this order:\n{0}{2}\n\n{3}",
                                 callDelimiter,
                                 formatter.FormatQuery(),
                                 formatter.FormatActualCalls(),
                                 "*** Note: calls to property getters are not considered part of the query. ***");
        }
    }
}