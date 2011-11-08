using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class ReceivedCallsExceptionThrower : IReceivedCallsExceptionThrower
    {
        private readonly ICallFormatter _callFormatter;

        public ReceivedCallsExceptionThrower(ICallFormatter callFormatter)
        {
            _callFormatter = callFormatter;
        }

        public void Throw(ICallSpecification callSpecification, IEnumerable<ICall> matchingCalls, IEnumerable<ICall> relatedCalls, Quantity requiredQuantity)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("Expected to receive {0} matching:\n\t{1}", requiredQuantity.Describe("call", "calls"), callSpecification.Format(_callFormatter)));

            AppendMatchingCalls(callSpecification, matchingCalls, builder);

            if (requiredQuantity.IsMoreThan(matchingCalls))
            {
                AppendRelatedCalls(callSpecification, relatedCalls, builder);
            }
            throw new ReceivedCallsException(builder.ToString());
        }

        private void AppendRelatedCalls(ICallSpecification callSpecification, IEnumerable<ICall> relatedCalls, StringBuilder builder)
        {
            if (relatedCalls.Any())
            {
                var numberOfRelatedCalls = relatedCalls.Count();
                builder.AppendLine(
                    string.Format(
                        "Received {0} related {1} (non-matching arguments indicated with '*' characters):",
                        numberOfRelatedCalls,
                        numberOfRelatedCalls == 1 ? "call" : "calls")
                    );
                WriteCallsWithRespectToCallSpec(callSpecification, relatedCalls, builder);
            }
        }

        private void AppendMatchingCalls(ICallSpecification callSpecification, IEnumerable<ICall> matchingCalls, StringBuilder builder)
        {
            var numberOfMatchingCalls = matchingCalls.Count();
            if (numberOfMatchingCalls == 0)
            {
                builder.AppendLine("Actually received no matching calls.");
            }
            else
            {
                builder.AppendLine(string.Format("Actually received {0} matching {1}:", numberOfMatchingCalls, numberOfMatchingCalls == 1 ? "call" : "calls"));
                WriteCallsWithRespectToCallSpec(callSpecification, matchingCalls, builder);
            }
        }

        private void WriteCallsWithRespectToCallSpec(ICallSpecification callSpecification, IEnumerable<ICall> relatedCalls, StringBuilder builder)
        {
            foreach (var call in relatedCalls)
            {
                builder.AppendFormat("\t{0}\n", _callFormatter.Format(call, callSpecification));
            }
        }
    }
}