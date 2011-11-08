using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IReceivedCallsExceptionThrower
    {
        void Throw(ICallSpecification callSpecification, IEnumerable<ICall> matchingCalls, IEnumerable<ICall> relatedCalls, Quantity requiredQuantity);
    }
}