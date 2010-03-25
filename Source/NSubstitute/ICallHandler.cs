using System.Collections.Generic;

namespace NSubstitute
{
    public interface ICallHandler
    {
        void LastCallShouldReturn<T>(T valueToReturn, IList<IArgumentMatcher> argumentMatchers);
        object Handle(ICall call, IList<IArgumentMatcher> argumentMatchers);
        void AssertNextCallHasBeenReceived();
    }
}