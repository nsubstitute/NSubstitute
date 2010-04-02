using System.Collections.Generic;

namespace NSubstitute
{
    public interface ICallHandler
    {
        void LastCallShouldReturn<T>(T valueToReturn);
        object Handle(ICall call);
        void AssertNextCallHasBeenReceived();
        void RaiseEventFromNextCall(params object[] argumentsToRaiseEventWith);
    }
}