using System;

namespace NSubstitute.Core
{
    public interface ICallResults
    {
        void SetResult(ICallSpecification callSpecification, IReturn result);
        object GetResult(ICall call);
        object GetDefaultResultFor(ICall call);
    }
}