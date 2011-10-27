using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallNotReceivedExactlyExceptionThrower : ICallNotReceivedExactlyExceptionThrower
    {
        private readonly ICallFormatter _callFormatter;

        public CallNotReceivedExactlyExceptionThrower(ICallFormatter callFormatter)
        {
            _callFormatter = callFormatter;
        }

        public void Throw(ICallSpecification callSpecification, IEnumerable<ICall> actualCalls, int expectedCount)
        {
            var builder = new StringBuilder();
            int actualCount = actualCalls.Count();
            builder.AppendLine(string.Format("Expected to receive call {0} times but received {1} times:\n\t {0}", expectedCount, actualCount, callSpecification.Format(_callFormatter)));
            if (actualCount != expectedCount)
            {
                builder.AppendLine(string.Format("Actually received {0} calls that resemble the expected call.", actualCalls));
            }
            else
            {
                builder.AppendLine("Actually received (non-matching arguments indicated with '*' characters):");
                foreach (var call in actualCalls)
                {
                    builder.AppendFormat("\t{0}\n", _callFormatter.Format(call, callSpecification));
                }
            }
            throw new CallNotReceivedException(builder.ToString());
        }
    }
}