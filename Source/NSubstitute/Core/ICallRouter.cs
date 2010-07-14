using System.Collections.Generic;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public interface ICallRouter
    {
        void LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs);
        object Route(ICall call);
        void SetRoute<TRoute>(params object[] routeArguments) where TRoute : IRoute;
        void ClearReceivedCalls();
        IEnumerable<ICall> ReceivedCalls();
    }
}