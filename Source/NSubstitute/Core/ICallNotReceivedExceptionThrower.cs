using System;

namespace NSubstitute.Core
{
    public interface ICallNotReceivedExceptionThrower
    {
        void Throw(ICallSpecification callSpecification);
    }
}