using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallNotReceivedExceptionThrower : ICallNotReceivedExceptionThrower
    {
        public void Throw(ICallSpecification callSpecification)
        {
            throw new CallNotReceivedException("Expected call to " + callSpecification.MethodInfo.Name);
        }
    }
}