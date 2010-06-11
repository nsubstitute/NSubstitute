using System;
using NSubstitute.Routes;

namespace NSubstitute.Core
{
    public class CallRouter : ICallRouter
    {
        readonly ISubstitutionContext _context;
        private readonly IReceivedCalls _receivedCalls;
        readonly IResultSetter _resultSetter;
        IRoute _currentRoute;
        IRouteFactory _routeFactory;

        public CallRouter(ISubstitutionContext context, IReceivedCalls receivedCalls, IResultSetter resultSetter, IRouteFactory routeFactory)
        {
            _context = context;
            _receivedCalls = receivedCalls;
            _resultSetter = resultSetter;
            _routeFactory = routeFactory;

            UseDefaultRouteForNextCall();
        }

        public void SetRoute<TRoute>(params object[] routeArguments) where TRoute : IRoute
        {
            _currentRoute = _routeFactory.Create<TRoute>(routeArguments);
        }

        public void ClearReceivedCalls()
        {
            _receivedCalls.Clear();
        }

        private void UseDefaultRouteForNextCall()
        {
            SetRoute<RecordReplayRoute>();
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            var routeToUseForThisCall = _currentRoute;
            UseDefaultRouteForNextCall();
            return routeToUseForThisCall.Handle(call);
        }

        public void LastCallShouldReturn(IReturn returnValue)
        {
            _resultSetter.SetResultForLastCall(returnValue);
        }
    }
}