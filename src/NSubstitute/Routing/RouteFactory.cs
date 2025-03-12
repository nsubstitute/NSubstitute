using NSubstitute.Core;
using NSubstitute.ReceivedExtensions;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing;

public class RouteFactory(SequenceNumberGenerator sequenceNumberGenerator,
    IThreadLocalContext threadLocalContext,
    ICallSpecificationFactory callSpecificationFactory,
    IReceivedCallsExceptionThrower receivedCallsExceptionThrower,
    IPropertyHelper propertyHelper,
    IDefaultForType defaultForType) : IRouteFactory
{
    public IRoute CallQuery(ISubstituteState state)
    {
        return new Route([
            new ClearUnusedCallSpecHandler(threadLocalContext.PendingSpecification),
            new AddCallToQueryResultHandler(threadLocalContext),
            new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, callSpecificationFactory),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }
    public IRoute CheckReceivedCalls(ISubstituteState state, MatchArgs matchArgs, Quantity requiredQuantity)
    {
        return new Route([
            new ClearLastCallRouterHandler(threadLocalContext),
            new ClearUnusedCallSpecHandler(threadLocalContext.PendingSpecification),
            new CheckReceivedCallsHandler(state.ReceivedCalls, callSpecificationFactory, receivedCallsExceptionThrower, matchArgs, requiredQuantity),
            new ReturnAutoValue(AutoValueBehaviour.ReturnAndForgetValue, state.AutoValueProviders, state.AutoValuesCallResults, callSpecificationFactory),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }
    public IRoute DoWhenCalled(ISubstituteState state, Action<CallInfo> doAction, MatchArgs matchArgs)
    {
        return new Route([
            new ClearLastCallRouterHandler(threadLocalContext),
            new ClearUnusedCallSpecHandler(threadLocalContext.PendingSpecification),
            new SetActionForCallHandler(callSpecificationFactory, state.CallActions, doAction, matchArgs),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }
    public IRoute DoNotCallBase(ISubstituteState state, MatchArgs matchArgs)
    {
        return new Route([
            new ClearLastCallRouterHandler(threadLocalContext),
            new ClearUnusedCallSpecHandler(threadLocalContext.PendingSpecification),
            new DoNotCallBaseForCallHandler(callSpecificationFactory, state.CallBaseConfiguration, matchArgs),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }
    public IRoute CallBase(ISubstituteState state, MatchArgs matchArgs)
    {
        return new Route([
            new ClearLastCallRouterHandler(threadLocalContext),
            new ClearUnusedCallSpecHandler(threadLocalContext.PendingSpecification),
            new CallBaseForCallHandler(callSpecificationFactory, state.CallBaseConfiguration, matchArgs),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }
    public IRoute RaiseEvent(ISubstituteState state, Func<ICall, object?[]> getEventArguments)
    {
        return new Route([
            new ClearLastCallRouterHandler(threadLocalContext),
            new ClearUnusedCallSpecHandler(threadLocalContext.PendingSpecification),
            new RaiseEventHandler(state.EventHandlerRegistry, getEventArguments),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }
    public IRoute RecordCallSpecification(ISubstituteState state)
    {
        return new Route([
            new RecordCallSpecificationHandler(threadLocalContext.PendingSpecification, callSpecificationFactory, state.CallActions),
            new PropertySetterHandler(propertyHelper, state.ConfigureCall),
            new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, callSpecificationFactory),
            new ReturnFromAndConfigureDynamicCall(state.ConfigureCall),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }
    public IRoute RecordReplay(ISubstituteState state)
    {
        return new Route([
            new TrackLastCallHandler(threadLocalContext.PendingSpecification),
            new RecordCallHandler(state.ReceivedCalls, sequenceNumberGenerator),
            new EventSubscriptionHandler(state.EventHandlerRegistry),
            new PropertySetterHandler(propertyHelper, state.ConfigureCall),
            new DoActionsCallHandler(state.CallActions),
            new ReturnConfiguredResultHandler(state.CallResults),
            new ReturnResultForTypeHandler(state.ResultsForType),
            new ReturnFromBaseIfRequired(state.CallBaseConfiguration),
            new ReturnFromCustomHandlers(state.CustomHandlers),
            new ReturnAutoValue(AutoValueBehaviour.UseValueForSubsequentCalls, state.AutoValueProviders, state.AutoValuesCallResults, callSpecificationFactory),
            new ReturnFromAndConfigureDynamicCall(state.ConfigureCall),
            ReturnDefaultForReturnTypeHandler()
        ]);
    }

    private ReturnDefaultForReturnTypeHandler ReturnDefaultForReturnTypeHandler() => new(defaultForType);
}