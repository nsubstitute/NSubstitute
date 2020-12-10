using System;

namespace NSubstitute.Core
{
    public interface IResultsForType {
        void SetResult(Type type, IReturn resultToReturn);
        bool TryGetResult(ICall call, out object? result);
        void Clear();
    }
}