using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallNotReceivedExceptionThrower : ICallNotReceivedExceptionThrower
    {
        private readonly ICallFormatter _callFormatter;

        public CallNotReceivedExceptionThrower(ICallFormatter callFormatter)
        {
            _callFormatter = callFormatter;
        }

        public void Throw(ICallSpecification callSpecification, IEnumerable<ICall> actualCalls)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Expected to receive call:\n\t" + callSpecification.Format(_callFormatter));
            if (!actualCalls.Any())
            {
                builder.AppendLine("Actually received no calls that resemble the expected call.");
            }
            else
            {
                builder.AppendLine("Actually received:");
                foreach (var call in actualCalls)
                {
                    builder.AppendFormat("\t{0}\n", _callFormatter.Format(call));
                }
            }
            throw new CallNotReceivedException(builder.ToString());
        }
    }
}