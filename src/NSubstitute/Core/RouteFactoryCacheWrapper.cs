using NSubstitute.ReceivedExtensions;
using NSubstitute.Routing;

namespace NSubstitute.Core;

public class RouteFactoryCacheWrapper(IRouteFactory factory) : IRouteFactory
{
    private CachedRoute _recordReplayCache;
    private CachedRoute _recordCallSpecificationCache;

    public IRoute RecordReplay(ISubstituteState state)
    {
        // Don't care about concurrency - routes are immutable and in worst case we'll simply create a few ones.
        if (_recordReplayCache.State != state)
        {
            _recordReplayCache = new CachedRoute(factory.RecordReplay(state), state);
        }

        return _recordReplayCache.Route;
    }

    public IRoute RecordCallSpecification(ISubstituteState state)
    {
        // Don't care about concurrency - routes are immutable and in worst case we'll simply create a few ones.
        if (_recordCallSpecificationCache.State != state)
        {
            _recordCallSpecificationCache = new CachedRoute(factory.RecordCallSpecification(state), state);
        }

        return _recordCallSpecificationCache.Route;
    }

    public IRoute CallQuery(ISubstituteState state) =>
        factory.CallQuery(state);

    public IRoute CheckReceivedCalls(ISubstituteState state, MatchArgs matchArgs, Quantity requiredQuantity) =>
        factory.CheckReceivedCalls(state, matchArgs, requiredQuantity);

    public IRoute DoWhenCalled(ISubstituteState state, Action<CallInfo> doAction, MatchArgs matchArgs) =>
        factory.DoWhenCalled(state, doAction, matchArgs);

    public IRoute DoNotCallBase(ISubstituteState state, MatchArgs matchArgs) =>
        factory.DoNotCallBase(state, matchArgs);

    public IRoute CallBase(ISubstituteState state, MatchArgs matchArgs) =>
        factory.CallBase(state, matchArgs);

    public IRoute RaiseEvent(ISubstituteState state, Func<ICall, object?[]> getEventArguments) =>
        factory.RaiseEvent(state, getEventArguments);

    private readonly struct CachedRoute(IRoute route, ISubstituteState state)
    {
        public readonly IRoute Route = route;
        public readonly ISubstituteState State = state;
    }
}