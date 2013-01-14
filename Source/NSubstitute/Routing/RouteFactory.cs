using System;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing
{
    public class RouteFactory : IRouteFactory
    {
        public IRoute CallQuery(ISubstituteState state)
        {
            return new Route(new ICallHandler[] {
                new ClearUnusedCallSpecHandler(state.PendingSpecification)
                , new AddCallToQueryResultHandler(state.SubstitutionContext, state.CallSpecificationFactory)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnAutoValueForThisAndSubsequentCallsHandler(state.AutoValueProviders, state.ResultSetter)
                , new ReturnDefaultForReturnTypeHandler(state.DefaultForType)
            });
        }
        public IRoute CheckReceivedCalls(ISubstituteState state, MatchArgs matchArgs, Quantity requiredQuantity)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(state.SubstitutionContext)
                , new ClearUnusedCallSpecHandler(state.PendingSpecification)
                , new CheckReceivedCallsHandler(state.ReceivedCalls, state.CallSpecificationFactory, state.ReceivedCallsExceptionThrower, matchArgs, requiredQuantity)
                , new ReturnDefaultForReturnTypeHandler(state.DefaultForType)
            });
        }
        public IRoute DoWhenCalled(ISubstituteState state, Action<CallInfo> doAction, MatchArgs matchArgs)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(state.SubstitutionContext)
                , new ClearUnusedCallSpecHandler(state.PendingSpecification)
                , new SetActionForCallHandler(state.CallSpecificationFactory, state.CallActions, doAction, matchArgs)
                , new ReturnDefaultForReturnTypeHandler(state.DefaultForType)
            });
        }
        public IRoute RaiseEvent(ISubstituteState state, Func<ICall, object[]> getEventArguments)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(state.SubstitutionContext)
                , new ClearUnusedCallSpecHandler(state.PendingSpecification)
                , new RaiseEventHandler(state.EventHandlerRegistry, getEventArguments)
                , new ReturnDefaultForReturnTypeHandler(state.DefaultForType)
            });
        }
        public IRoute RecordCallSpecification(ISubstituteState state)
        {
            return new Route(new ICallHandler[] {
                new RecordCallSpecificationHandler(state.PendingSpecification, state.CallSpecificationFactory, state.CallActions)
                , new PropertySetterHandler(state.PropertyHelper, state.ResultSetter)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnAutoValueForThisAndSubsequentCallsHandler(state.AutoValueProviders, state.ResultSetter)
                , new ReturnDefaultForReturnTypeHandler(state.DefaultForType)
            });
        }
        public IRoute RecordReplay(ISubstituteState state)
        {
            return new Route(RouteType.RecordReplay, new ICallHandler[] {
                new ClearUnusedCallSpecHandler(state.PendingSpecification)
                , new RecordCallHandler(state.CallStack, state.SequenceNumberGenerator)
                , new EventSubscriptionHandler(state.EventHandlerRegistry)
                , new PropertySetterHandler(state.PropertyHelper, state.ResultSetter)
                , new DoActionsCallHandler(state.CallActions)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnAutoValueForThisAndSubsequentCallsHandler(state.AutoValueProviders, state.ResultSetter)
                , new ReturnDefaultForReturnTypeHandler(state.DefaultForType)
            });
        }
    }
}