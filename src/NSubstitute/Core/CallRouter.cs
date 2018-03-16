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
        readonly IThreadLocalContext _threadContext;
        readonly IRouteFactory _routeFactory;
        IRoute _currentRoute;

        public CallRouter(ISubstituteState substituteState, IThreadLocalContext threadContext, IRouteFactory routeFactory)
        {
            _substituteState = substituteState;
            _threadContext = threadContext;
            _routeFactory = routeFactory;

            UseDefaultRouteForNextCall();
        }

        public void SetRoute(Func<ISubstituteState, IRoute> routeFactory)
        {
            _currentRoute = routeFactory.Invoke(_substituteState);
        }

        public void Clear(ClearOptions options)
        {
            if ((options & ClearOptions.CallActions) == ClearOptions.CallActions)
            {
                _substituteState.CallActions.Clear();
            }
            if ((options & ClearOptions.ReturnValues) == ClearOptions.ReturnValues)
            {
                _substituteState.CallResults.Clear();
                _substituteState.ResultsForType.Clear();
            }
            if ((options & ClearOptions.ReceivedCalls) == ClearOptions.ReceivedCalls)
            {
                _substituteState.ReceivedCalls.Clear();
            }
        }
        public IEnumerable<ICall> ReceivedCalls()
        {
            return _substituteState.ReceivedCalls.AllCalls();
        }

        public object Route(ICall call)
        {
            _threadContext.SetLastCallRouter(this);

            var pendingRaisingEventArgs = _threadContext.UsePendingRaisingEventArgumentsFactory();

            IRoute routeToUseForThisCall;
            if (_threadContext.IsQuerying)
            {
                routeToUseForThisCall = GetQueryRoute();
            }
            else if (pendingRaisingEventArgs != null)
            {
                routeToUseForThisCall = GetRaiseEventRoute(pendingRaisingEventArgs);
            }
            else if (IsSpecifyingACall(call, _currentRoute))
            {
                routeToUseForThisCall = GetRecordCallSpecRoute();
            }
            else
            {
                routeToUseForThisCall = _currentRoute;
            }

            UseDefaultRouteForNextCall();
            return routeToUseForThisCall.Handle(call);
        }

        private IRoute GetQueryRoute()
        {
            return _routeFactory.CallQuery(_substituteState);
        }

        private IRoute GetRaiseEventRoute(Func<ICall, object[]> argumentsFactory)
        {
            return _routeFactory.RaiseEvent(_substituteState, argumentsFactory);
        }

        private static bool IsSpecifyingACall(ICall call, IRoute currentRoute)
        {
            var args = call.GetOriginalArguments() ?? EmptyArgs;
            var argSpecs = call.GetArgumentSpecifications() ?? EmptyArgSpecs;
            return currentRoute.IsRecordReplayRoute && args.Any() && argSpecs.Any();
        }

        private IRoute GetRecordCallSpecRoute()
        {
            return _routeFactory.RecordCallSpecification(_substituteState);
        }

        private void UseDefaultRouteForNextCall()
        {
            SetRoute(x => _routeFactory.RecordReplay(x));
        }

        public bool IsLastCallInfoPresent()
        {
            return _threadContext.PendingSpecification.HasPendingCallSpecInfo();
        }

        public ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs)
        {
            return _substituteState.ConfigureCall.SetResultForLastCall(returnValue, matchArgs, _threadContext.PendingSpecification);
        }

        public void SetReturnForType(Type type, IReturn returnValue)
        {
            _substituteState.ResultsForType.SetResult(type, returnValue);
        }

        public void RegisterCustomCallHandlerFactory(CallHandlerFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            _substituteState.CustomHandlers.AddCustomHandlerFactory(factory);
        }
    }
}