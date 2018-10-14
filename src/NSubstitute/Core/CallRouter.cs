using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouter : ICallRouter
    {
        private readonly ISubstituteState _substituteState;
        private readonly IThreadLocalContext _threadContext;
        private readonly IRouteFactory _routeFactory;
        private readonly bool _canConfigureBaseCalls;

        public CallRouter(ISubstituteState substituteState, IThreadLocalContext threadContext, IRouteFactory routeFactory, bool canConfigureBaseCalls)
        {
            _substituteState = substituteState;
            _threadContext = threadContext;
            _routeFactory = routeFactory;
            _canConfigureBaseCalls = canConfigureBaseCalls;
        }

        public bool CallBaseByDefault
        {
            get => _substituteState.CallBaseConfiguration.CallBaseByDefault;
            set
            {
                if (!_canConfigureBaseCalls) throw CouldNotConfigureCallBaseException.ForAllCalls();

                _substituteState.CallBaseConfiguration.CallBaseByDefault = value;
            }
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

        public void SetRoute(Func<ISubstituteState, IRoute> getRoute) => 
            _threadContext.SetNextRoute(this, getRoute);

        public object Route(ICall call)
        {
            _threadContext.SetLastCallRouter(this);

            var pendingRaisingEventArgs = _threadContext.UsePendingRaisingEventArgumentsFactory();
            var queuedNextRouteFactory = _threadContext.UseNextRoute(this);

            IRoute routeToUseForThisCall;
            if (_threadContext.IsQuerying)
            {
                routeToUseForThisCall = GetQueryRoute();
            }
            else if (pendingRaisingEventArgs != null)
            {
                routeToUseForThisCall = GetRaiseEventRoute(pendingRaisingEventArgs);
            }
            else if (queuedNextRouteFactory != null)
            {
                routeToUseForThisCall = queuedNextRouteFactory.Invoke(_substituteState);
            }
            else if (IsSpecifyingACall(call))
            {
                routeToUseForThisCall = GetRecordCallSpecRoute();
            }
            else
            {
                routeToUseForThisCall = GetRecordReplayRoute();
            }

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

        private IRoute GetRecordCallSpecRoute()
        {
            return _routeFactory.RecordCallSpecification(_substituteState);
        }

        private IRoute GetRecordReplayRoute()
        {
            return _routeFactory.RecordReplay(_substituteState);
        }

        private static bool IsSpecifyingACall(ICall call)
        {
            return call.GetOriginalArguments().Length != 0 && call.GetArgumentSpecifications().Count != 0;
        }

        public ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo)
        {
            if (returnValue == null) throw new ArgumentNullException(nameof(returnValue));
            if (matchArgs == null) throw new ArgumentNullException(nameof(matchArgs));
            if (pendingSpecInfo == null) throw new ArgumentNullException(nameof(pendingSpecInfo));

            return _substituteState.ConfigureCall.SetResultForLastCall(returnValue, matchArgs, pendingSpecInfo);
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