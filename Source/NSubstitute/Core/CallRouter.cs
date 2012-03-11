using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute.Core.Arguments;
using NSubstitute.Routing;
using NSubstitute.Routing.Definitions;

namespace NSubstitute.Core
{
    public class CallRouter : ICallRouter
    {
        static readonly object[] EmptyArgs = new object[0];
        static readonly IList<IArgumentSpecification> EmptyArgSpecs = new List<IArgumentSpecification>();
        readonly ISubstitutionContext _context;
        readonly IReceivedCalls _receivedCalls;
        readonly IResultSetter _resultSetter;
        readonly IRouteFactory _routeFactory;
        readonly ICallSpecificationFactory _callSpecificationFactory;
        IRoute _currentRoute;
        bool _isSetToDefaultRoute;

        public CallRouter(ISubstitutionContext context, IReceivedCalls receivedCalls, IResultSetter resultSetter, IRouteFactory routeFactory, ICallSpecificationFactory callSpecificationFactory)
        {
            _context = context;
            _receivedCalls = receivedCalls;
            _resultSetter = resultSetter;
            _routeFactory = routeFactory;
            _callSpecificationFactory = callSpecificationFactory;

            UseDefaultRouteForNextCall();
        }

        public void SetRoute<TRouteDefinition>(params object[] routeArguments) where TRouteDefinition : IRouteDefinition
        {
            _isSetToDefaultRoute = typeof(TRouteDefinition) == typeof(RecordReplayRoute);
            _currentRoute = _routeFactory.Create<TRouteDefinition>(routeArguments);
        }

        public void ClearReceivedCalls()
        {
            _receivedCalls.Clear();
        }

        public IEnumerable<ICall> ReceivedCalls()
        {
            return _receivedCalls.AllCalls();
        }

        public IEnumerable<ICall> ReceivedCalls<T>(Expression<Action<T>> call)
        {
            return ReceivedCalls().Where(c => IsSatisfiedBy(c, call));
        }

        private bool IsSatisfiedBy<T>(ICall actualCall, Expression<Action<T>> expectedCall)
        {
            ICallSpecification callSpecification = _callSpecificationFactory.CreateFrom(expectedCall);

            return callSpecification.IsSatisfiedBy(actualCall);
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            if (IsSpecifyingACall(call)) { UseRecordCallSpecRouteForNextCall(); }
            var routeToUseForThisCall = _currentRoute;
            UseDefaultRouteForNextCall();
            return routeToUseForThisCall.Handle(call);
        }

        public void LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs)
        {
            _resultSetter.SetResultForLastCall(returnValue, matchArgs);
        }

        private bool IsSpecifyingACall(ICall call)
        {
            var args = call.GetArguments() ?? EmptyArgs;
            var argSpecs = call.GetArgumentSpecifications() ?? EmptyArgSpecs;
            return _isSetToDefaultRoute && args.Any() && argSpecs.Any();
        }

        private void UseDefaultRouteForNextCall()
        {
            SetRoute<RecordReplayRoute>();
        }

        private void UseRecordCallSpecRouteForNextCall()
        {
            SetRoute<RecordCallSpecificationRoute>();
        }
    }
}