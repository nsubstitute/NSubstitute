using System;
using System.Collections.Generic;
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

        public object? Route(ICall call)
        {
            _threadContext.SetLastCallRouter(this);

            var isQuerying = _threadContext.IsQuerying;
            var pendingRaisingEventArgs = _threadContext.UsePendingRaisingEventArgumentsFactory();
            var queuedNextRouteFactory = _threadContext.UseNextRoute(this);

            IRoute routeToUse = ResolveCurrentRoute(call, isQuerying, pendingRaisingEventArgs, queuedNextRouteFactory);

            return routeToUse.Handle(call);
        }

        private IRoute ResolveCurrentRoute(ICall call, bool isQuerying, Func<ICall, object?[]>? pendingRaisingEventArgs, Func<ISubstituteState, IRoute>? queuedNextRouteFactory)
        {
            if (isQuerying)
            {
                return _routeFactory.CallQuery(_substituteState);
            }

            if (pendingRaisingEventArgs != null)
            {
                return _routeFactory.RaiseEvent(_substituteState, pendingRaisingEventArgs);
            }

            if (queuedNextRouteFactory != null)
            {
                return queuedNextRouteFactory.Invoke(_substituteState);
            }

            if (IsSpecifyingACall(call))
            {
                return _routeFactory.RecordCallSpecification(_substituteState);
            }

            return _routeFactory.RecordReplay(_substituteState);
        }

        private static bool IsSpecifyingACall(ICall call)
        {
            return call.GetOriginalArguments().Length != 0 && call.GetArgumentSpecifications().Count != 0;
        }

        public ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo)
        {
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