using System;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallReceivedExceptionThrower : ICallReceivedExceptionThrower
    {
        private readonly ICallFormatter _callFormatter;

        public CallReceivedExceptionThrower(ICallFormatter callFormatter)
        {
            _callFormatter = callFormatter;
        }

        public void Throw(ICallSpecification callSpecification)
        {
            throw new CallReceivedException("Expected: " + _callFormatter.Format(callSpecification.MethodInfo, callSpecification.ArgumentSpecifications.ToArray()));
        }
    }
}