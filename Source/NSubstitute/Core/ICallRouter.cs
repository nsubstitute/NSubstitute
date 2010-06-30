using System.Collections.Generic;

namespace NSubstitute.Core
{
    public interface ICallRouter
    {
        void LastCallShouldReturn(IReturn returnValue, bool matchLastCallsArguments);
        object Route(ICall call);
        void SetRoute<TRoute>(params object[] routeArguments) where TRoute : IRoute;
        void ClearReceivedCalls();
        IEnumerable<ICall> ReceivedCalls();
    }
}