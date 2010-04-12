using System.Collections.Generic;

namespace NSubstitute
{
    public interface ICallRouter
    {
        void LastCallShouldReturn<T>(T valueToReturn);
        object Route(ICall call);
        void AssertNextCallHasBeenReceived();
        void RaiseEventFromNextCall(params object[] argumentsToRaiseEventWith);
    }
}