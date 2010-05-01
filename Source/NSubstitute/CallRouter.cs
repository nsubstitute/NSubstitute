using System;
using NSubstitute.Routes;

namespace NSubstitute
{
    public class CallRouter : ICallRouter
    {
        readonly ISubstitutionContext _context;
        readonly IResultSetter _resultSetter;
        IRoute _currentRoute;
        IRouteFactory _routeFactory;

        public CallRouter(ISubstitutionContext context, IResultSetter resultSetter, IRouteFactory routeFactory)
        {
            _context = context;
            _resultSetter = resultSetter;
            _routeFactory = routeFactory;

            SetRoute<RecordReplayRoute>();
        }

        public void SetRoute<TRoute>(params object[] routeArguments) where TRoute : IRoute
        {
            _currentRoute = _routeFactory.Create<TRoute>(routeArguments);
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            var result = _currentRoute.Handle(call);
            SetRoute<RecordReplayRoute>();
            return result;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            _resultSetter.SetResultForLastCall(valueToReturn);
        }
    }
}