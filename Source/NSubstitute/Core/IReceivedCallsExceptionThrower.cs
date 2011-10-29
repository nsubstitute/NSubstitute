using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IReceivedCallsExceptionThrower
    {
        void Throw(ICallSpecification callSpecification);
        void Throw(ICallSpecification callSpecification, IEnumerable<ICall> actualCalls, Quantity requiredQuantity);
        void Throw(ICallSpecification callSpecification, IEnumerable<ICall> actualCalls, int expectedCount);
    }
}