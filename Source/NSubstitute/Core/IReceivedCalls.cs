using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IReceivedCalls
    {
        void ThrowIfCallNotFound(ICallSpecification callSpecification);
        IEnumerable<ICall> FindMatchingCalls(ICallSpecification callSpecification);
    }
}