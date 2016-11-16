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
                new ClearUnusedCallSpecHandler(state)
                , new AddCallToQueryResultHandler(state.SubstitutionContext, state.CallSpecificationFactory)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, state.CallSpecificationFactory)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute CheckReceivedCalls(ISubstituteState state, MatchArgs matchArgs, Quantity requiredQuantity)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(state.SubstitutionContext)
                , new ClearUnusedCallSpecHandler(state)
                , new CheckReceivedCallsHandler(state.ReceivedCalls, state.CallSpecificationFactory, new ReceivedCallsExceptionThrower(), matchArgs, requiredQuantity)
                , new ReturnAutoValue(AutoValueBehaviour.ReturnAndForgetValue, state.AutoValueProviders, state.AutoValuesCallResults, state.CallSpecificationFactory)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute DoWhenCalled(ISubstituteState state, Action<CallInfo> doAction, MatchArgs matchArgs)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(state.SubstitutionContext)
                , new ClearUnusedCallSpecHandler(state)
                , new SetActionForCallHandler(state.CallSpecificationFactory, state.CallActions, doAction, matchArgs)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute DoNotCallBase(ISubstituteState state, MatchArgs matchArgs)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(state.SubstitutionContext)
                , new ClearUnusedCallSpecHandler(state)
                , new DoNotCallBaseForCallHandler(state.CallSpecificationFactory, state.CallBaseExclusions, matchArgs)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute RaiseEvent(ISubstituteState state, Func<ICall, object[]> getEventArguments)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(state.SubstitutionContext)
                , new ClearUnusedCallSpecHandler(state)
                , new RaiseEventHandler(state.EventHandlerRegistry, getEventArguments)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute RecordCallSpecification(ISubstituteState state)
        {
            return new Route(new ICallHandler[] {
                new RecordCallSpecificationHandler(state.PendingSpecification, state.CallSpecificationFactory, state.CallActions)
                , new PropertySetterHandler(new PropertyHelper(), state.ConfigureCall)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, state.CallSpecificationFactory)
                , new ReturnFromAndConfigureDynamicCall(state.ConfigureCall)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute RecordReplay(ISubstituteState state)
        {
            return new Route(RouteType.RecordReplay, new ICallHandler[] {
                new ClearUnusedCallSpecHandler(state)
                , new RecordCallHandler(state.CallStack, state.SequenceNumberGenerator)
                , new EventSubscriptionHandler(state.EventHandlerRegistry)
                , new PropertySetterHandler(new PropertyHelper(), state.ConfigureCall)
                , new DoActionsCallHandler(state.CallActions)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnResultForTypeHandler(state.ResultsForType)
                , new ReturnFromBaseIfRequired(state.SubstituteConfig, state.CallBaseExclusions)
                , new ReturnFromCustomHandlers(state.CustomHandlers)
                , new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, state.CallSpecificationFactory)
                , new ReturnFromAndConfigureDynamicCall(state.ConfigureCall)
                , ReturnDefaultForReturnTypeHandler()
            });
        }

        private static ReturnDefaultForReturnTypeHandler ReturnDefaultForReturnTypeHandler()
        {
            return new ReturnDefaultForReturnTypeHandler(new DefaultForType());
        }
    }
}