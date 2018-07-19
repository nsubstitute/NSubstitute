using System;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.ReceivedExtensions;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing
{
    public class RouteFactory : IRouteFactory
    {
        private readonly SequenceNumberGenerator _sequenceNumberGenerator;
        private readonly IThreadLocalContext _threadLocalContext;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public RouteFactory(SequenceNumberGenerator sequenceNumberGenerator, IThreadLocalContext threadLocalContext, ICallSpecificationFactory callSpecificationFactory)
        {
            _sequenceNumberGenerator = sequenceNumberGenerator ?? throw new ArgumentNullException(nameof(sequenceNumberGenerator));
            _threadLocalContext = threadLocalContext ?? throw new ArgumentNullException(nameof(threadLocalContext));
            _callSpecificationFactory = callSpecificationFactory ?? throw new ArgumentNullException(nameof(callSpecificationFactory));
        }

        public IRoute CallQuery(ISubstituteState state)
        {
            return new Route(new ICallHandler[] {
                new ClearUnusedCallSpecHandler(_threadLocalContext.PendingSpecification)
                , new AddCallToQueryResultHandler(_threadLocalContext)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, _callSpecificationFactory)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute CheckReceivedCalls(ISubstituteState state, MatchArgs matchArgs, Quantity requiredQuantity)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(_threadLocalContext)
                , new ClearUnusedCallSpecHandler(_threadLocalContext.PendingSpecification)
                , new CheckReceivedCallsHandler(state.ReceivedCalls, _callSpecificationFactory, new ReceivedCallsExceptionThrower(), matchArgs, requiredQuantity)
                , new ReturnAutoValue(AutoValueBehaviour.ReturnAndForgetValue, state.AutoValueProviders, state.AutoValuesCallResults, _callSpecificationFactory)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute DoWhenCalled(ISubstituteState state, Action<CallInfo> doAction, MatchArgs matchArgs)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(_threadLocalContext)
                , new ClearUnusedCallSpecHandler(_threadLocalContext.PendingSpecification)
                , new SetActionForCallHandler(_callSpecificationFactory, state.CallActions, doAction, matchArgs)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute DoNotCallBase(ISubstituteState state, MatchArgs matchArgs)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(_threadLocalContext)
                , new ClearUnusedCallSpecHandler(_threadLocalContext.PendingSpecification)
                , new DoNotCallBaseForCallHandler(_callSpecificationFactory, state.CallBaseConfiguration, matchArgs)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute RaiseEvent(ISubstituteState state, Func<ICall, object[]> getEventArguments)
        {
            return new Route(new ICallHandler[] {
                new ClearLastCallRouterHandler(_threadLocalContext)
                , new ClearUnusedCallSpecHandler(_threadLocalContext.PendingSpecification)
                , new RaiseEventHandler(state.EventHandlerRegistry, getEventArguments)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute RecordCallSpecification(ISubstituteState state)
        {
            return new Route(new ICallHandler[] {
                new RecordCallSpecificationHandler(_threadLocalContext.PendingSpecification, _callSpecificationFactory, state.CallActions)
                , new PropertySetterHandler(new PropertyHelper(new CallFactory(), new ArgumentSpecificationCompatibilityTester(new DefaultChecker(new DefaultForType()))), state.ConfigureCall)
                , new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, _callSpecificationFactory)
                , new ReturnFromAndConfigureDynamicCall(state.ConfigureCall)
                , ReturnDefaultForReturnTypeHandler()
            });
        }
        public IRoute RecordReplay(ISubstituteState state)
        {
            return new Route(RouteType.RecordReplay, new ICallHandler[] {
                new ClearUnusedCallSpecHandler(_threadLocalContext.PendingSpecification)
                , new TrackLastCallHandler(_threadLocalContext.PendingSpecification)
                , new RecordCallHandler(state.ReceivedCalls, _sequenceNumberGenerator)
                , new EventSubscriptionHandler(state.EventHandlerRegistry)
                , new PropertySetterHandler(new PropertyHelper(new CallFactory(), new ArgumentSpecificationCompatibilityTester(new DefaultChecker(new DefaultForType()))), state.ConfigureCall)
                , new DoActionsCallHandler(state.CallActions)
                , new ReturnConfiguredResultHandler(state.CallResults)
                , new ReturnResultForTypeHandler(state.ResultsForType)
                , new ReturnFromBaseIfRequired(state.CallBaseConfiguration)
                , new ReturnFromCustomHandlers(state.CustomHandlers)
                , new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, _callSpecificationFactory)
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