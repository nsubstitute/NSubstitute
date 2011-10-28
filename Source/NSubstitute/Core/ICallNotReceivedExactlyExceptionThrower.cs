using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface ICallNotReceivedExactlyExceptionThrower
    {
        void Throw(ICallSpecification callSpecification, IEnumerable<ICall> actualCalls, int expectedCount);
    }
}