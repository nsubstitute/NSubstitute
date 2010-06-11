using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IReceivedCalls
    {
        IEnumerable<ICall> FindMatchingCalls(ICallSpecification callSpecification);
        void Clear();
    }
}