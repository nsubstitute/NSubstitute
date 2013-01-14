using System;
using System.Collections.Generic;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public interface ICallRouter
    {
        void LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs);
        object Route(ICall call);
        void ClearReceivedCalls();
        IEnumerable<ICall> ReceivedCalls();
        void SetRoute(Func<ISubstituteState, IRoute> getRoute);
    }
}