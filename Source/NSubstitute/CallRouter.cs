using System;

namespace NSubstitute
{
    public class CallRouter : ICallRouter
    {
        readonly ISubstitutionContext _context;
        readonly IResultSetter _resultSetter;
        IRoute _currentRoute;
        private IRouteFactory _routeFactory;

        public CallRouter(ISubstitutionContext context, IResultSetter resultSetter, IRouteFactory routeFactory)
        {
            _context = context;
            _resultSetter = resultSetter;
            _routeFactory = routeFactory;

            RecordAndReplayOnNextCall();
        }

        public void SetRoute<TRoute>(params object[] routeArguments) where TRoute : IRoute
        {
            _currentRoute = _routeFactory.Create<TRoute>(routeArguments);
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            var result = _currentRoute.Handle(call);
            RecordAndReplayOnNextCall();
            return result;
        }

        private void RecordAndReplayOnNextCall()
        {
            SetRoute<RecordReplayRoute>();
        }

        public void AssertNextCallHasBeenReceived()
        {
            SetRoute<CheckCallReceivedRoute>();
        }

        public void RaiseEventFromNextCall(Func<ICall, object[]> argumentsToRaiseEventWith)
        {
            SetRoute<RaiseEventRoute>(argumentsToRaiseEventWith);
        }

        public void AddCallbackForNextCall(Action<object[]> callbackWithArguments)
        {
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            _resultSetter.SetResultForLastCall(valueToReturn);
        }
    }
}