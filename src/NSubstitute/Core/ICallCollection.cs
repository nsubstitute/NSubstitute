using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface ICallCollection
    {
        void Add(ICall call);
        void Delete(ICall call);
        IEnumerable<ICall> AllCalls();
        void Clear();
    }
}