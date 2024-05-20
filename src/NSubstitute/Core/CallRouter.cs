using NSubstitute.Exceptions;
using NSubstitute.Routing;

namespace NSubstitute.Core;

public class CallRouter(
    ISubstituteState substituteState,
    IThreadLocalContext threadContext,
    IRouteFactory routeFactory,
    bool canConfigureBaseCalls) : ICallRouter
{
    public bool CallBaseByDefault
    {
        get => substituteState.CallBaseConfiguration.CallBaseByDefault;
        set
        {
            if (!canConfigureBaseCalls) throw CouldNotConfigureCallBaseException.ForAllCalls();

            substituteState.CallBaseConfiguration.CallBaseByDefault = value;
        }
    }

    public void Clear(ClearOptions options)
    {
        if ((options & ClearOptions.CallActions) == ClearOptions.CallActions)
        {
            substituteState.CallActions.Clear();
        }
        if ((options & ClearOptions.ReturnValues) == ClearOptions.ReturnValues)
        {
            substituteState.CallResults.Clear();
            substituteState.ResultsForType.Clear();
        }
        if ((options & ClearOptions.ReceivedCalls) == ClearOptions.ReceivedCalls)
        {
            substituteState.ReceivedCalls.Clear();
        }
    }

    public IEnumerable<ICall> ReceivedCalls()
    {
        return substituteState.ReceivedCalls.AllCalls();
    }

    public void SetRoute(Func<ISubstituteState, IRoute> getRoute) =>
        threadContext.SetNextRoute(this, getRoute);

    public object? Route(ICall call)
    {
        threadContext.SetLastCallRouter(this);

        var isQuerying = threadContext.IsQuerying;
        var pendingRaisingEventArgs = threadContext.UsePendingRaisingEventArgumentsFactory();
        var queuedNextRouteFactory = threadContext.UseNextRoute(this);

        IRoute routeToUse = ResolveCurrentRoute(call, isQuerying, pendingRaisingEventArgs, queuedNextRouteFactory);

        return routeToUse.Handle(call);
    }

    private IRoute ResolveCurrentRoute(ICall call, bool isQuerying, Func<ICall, object?[]>? pendingRaisingEventArgs, Func<ISubstituteState, IRoute>? queuedNextRouteFactory)
    {
        if (isQuerying)
        {
            return routeFactory.CallQuery(substituteState);
        }

        if (pendingRaisingEventArgs != null)
        {
            return routeFactory.RaiseEvent(substituteState, pendingRaisingEventArgs);
        }

        if (queuedNextRouteFactory != null)
        {
            return queuedNextRouteFactory.Invoke(substituteState);
        }

        if (IsSpecifyingACall(call))
        {
            return routeFactory.RecordCallSpecification(substituteState);
        }

        return routeFactory.RecordReplay(substituteState);
    }

    private static bool IsSpecifyingACall(ICall call)
    {
        return call.GetOriginalArguments().Length != 0 && call.GetArgumentSpecifications().Count != 0;
    }

    public ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs, PendingSpecificationInfo pendingSpecInfo)
    {
        return substituteState.ConfigureCall.SetResultForLastCall(returnValue, matchArgs, pendingSpecInfo);
    }

    public void SetReturnForType(Type type, IReturn returnValue)
    {
        substituteState.ResultsForType.SetResult(type, returnValue);
    }

    public void RegisterCustomCallHandlerFactory(CallHandlerFactory factory)
    {
        if (factory == null) throw new ArgumentNullException(nameof(factory));

        substituteState.CustomHandlers.AddCustomHandlerFactory(factory);
    }
}