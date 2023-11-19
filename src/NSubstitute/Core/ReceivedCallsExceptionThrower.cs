using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.ReceivedExtensions;

namespace NSubstitute.Core
{
    public class ReceivedCallsExceptionThrower : IReceivedCallsExceptionThrower
    {
        public void Throw(ICallSpecification callSpecification, IEnumerable<ICall> matchingCalls, IEnumerable<ICall> nonMatchingCalls, Quantity requiredQuantity)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("Expected to receive {0} matching:\n\t{1}", requiredQuantity.Describe("call", "calls"), callSpecification));

            AppendMatchingCalls(callSpecification, matchingCalls, builder);

            if (requiredQuantity.RequiresMoreThan(matchingCalls))
            {
                AppendNonMatchingCalls(callSpecification, nonMatchingCalls, builder);
            }

            throw new ReceivedCallsException(builder.ToString());
        }

        private void AppendNonMatchingCalls(ICallSpecification callSpecification, IEnumerable<ICall> nonMatchingCalls, StringBuilder builder)
        {
            if (nonMatchingCalls.Any())
            {
                var numberOfRelatedCalls = nonMatchingCalls.Count();
                builder.AppendLine(
                    string.Format(
                        "Received {0} non-matching {1} (non-matching arguments indicated with '*' characters):",
                        numberOfRelatedCalls,
                        numberOfRelatedCalls == 1 ? "call" : "calls")
                    );
                WriteCallsWithRespectToCallSpec(callSpecification, nonMatchingCalls, builder);
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
                builder.AppendFormat("\t{0}\n", callSpecification.Format(call));
                var nonMatches = DescribeNonMatches(call, callSpecification).Trim();
                if (!string.IsNullOrEmpty(nonMatches))
                {
                    builder.AppendFormat("\t\t{0}\n", nonMatches.Replace("\n", "\n\t\t"));
                }
            }
        }

        private string DescribeNonMatches(ICall call, ICallSpecification callSpecification)
        {
            var nonMatchingArgDescriptions = callSpecification
                .NonMatchingArguments(call)
                .Select(x => x.DescribeNonMatch())
                .Where(x => !string.IsNullOrEmpty(x));
            return string.Join("\n", nonMatchingArgDescriptions);
        }
    }
}