using System.Collections.Generic;
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
            builder.AppendLine("Expected to receive call: " + callSpecification.Format(_callFormatter));
            builder.AppendLine("Actually received:");
            foreach (var call in actualCalls)
            {
                builder.AppendFormat("\t{0}\n", callSpecification.HowDifferentFrom(call, _callFormatter));
            }
            throw new CallNotReceivedException(builder.ToString());
        }
    }
}