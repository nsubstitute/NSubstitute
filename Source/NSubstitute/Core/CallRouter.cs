using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core.Arguments;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouter : ICallRouter
    {
        static readonly object[] EmptyArgs = new object[0];
        static readonly IList<IArgumentSpecification> EmptyArgSpecs = new List<IArgumentSpecification>();
        readonly ISubstituteState _substituteState;
        readonly ISubstitutionContext _context;
        readonly IReceivedCalls _receivedCalls;
        readonly IConfigureCall ConfigureCall;
        readonly IRouteFactory _routeFactory;
        IRoute _currentRoute;
        bool _isSetToDefaultRoute;

        public CallRouter(ISubstituteState substituteState, ISubstitutionContext context, IRouteFactory routeFactory)
        {
            _substituteState = substituteState;
            _context = context;
            _routeFactory = routeFactory;
            _receivedCalls = substituteState.ReceivedCalls;
            ConfigureCall = substituteState.ConfigureCall;

            UseDefaultRouteForNextCall();
        }

        public void SetRoute(Func<ISubstituteState, IRoute> getRoute)
        {
            var route = getRoute(_substituteState);
            _isSetToDefaultRoute = route.IsRecordReplayRoute;
            _currentRoute = route;
        }

        public void ClearReceivedCalls()
        {
            _receivedCalls.Clear();
        }

        public IEnumerable<ICall> ReceivedCalls()
        {
            return _receivedCalls.AllCalls();
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            if (_context.IsQuerying) { UseQueryRouteForNextCall(); }
            else if (IsSpecifyingACall(call)) { UseRecordCallSpecRouteForNextCall(); }

            var routeToUseForThisCall = _currentRoute;
            UseDefaultRouteForNextCall();
            return routeToUseForThisCall.Handle(call);
        }

        public ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs)
        {
            return ConfigureCall.SetResultForLastCall(returnValue, matchArgs);
        }

        private bool IsSpecifyingACall(ICall call)
        {
            var args = call.GetArguments() ?? EmptyArgs;
            var argSpecs = call.GetArgumentSpecifications() ?? EmptyArgSpecs;
            return _isSetToDefaultRoute && args.Any() && argSpecs.Any();
        }

        private void UseDefaultRouteForNextCall()
        {
            SetRoute(x => _routeFactory.RecordReplay(x));
        }

        private void UseRecordCallSpecRouteForNextCall()
        {
            SetRoute(x => _routeFactory.RecordCallSpecification(x));
        }

        private void UseQueryRouteForNextCall()
        {
            SetRoute(x => _routeFactory.CallQuery(x));
        }

        public void SetReturnForType(Type type, IReturn returnValue)
        {
            _substituteState.ResultsForType.SetResult(type, returnValue);
        }
    }
}