using System;

namespace NSubstitute.Core
{
    public interface IResultsForType {
        bool HasResultFor(ICall call);
        void SetResult(Type type, IReturn resultToReturn);
        object GetResult(ICall call);
        void Clear();
    }
}