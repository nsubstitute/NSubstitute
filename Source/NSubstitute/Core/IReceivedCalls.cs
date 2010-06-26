using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IReceivedCalls
    {
        IEnumerable<ICall> FindMatchingCalls(ICallSpecification callSpecification);
        IEnumerable<ICall> AllCalls();
        void Clear();
    }
}