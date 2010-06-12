using System;

namespace NSubstitute.Core
{
    public interface ICallReceivedExceptionThrower
    {
        void Throw(ICallSpecification callSpecification);
    }
}