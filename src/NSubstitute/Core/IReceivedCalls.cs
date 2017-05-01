using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface IReceivedCalls
    {
        IEnumerable<ICall> AllCalls();
        void Clear();
    }
}