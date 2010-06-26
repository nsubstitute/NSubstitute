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
            throw new CallReceivedException("Unexpected call to: " + callSpecification.Format(_callFormatter));
        }
    }
}