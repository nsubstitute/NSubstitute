using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface ICallNotReceivedExceptionThrower
    {
        void Throw(ICallSpecification callSpecification, IEnumerable<ICall> actualCalls);
    }
}