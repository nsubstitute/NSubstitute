using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NSubstitute.Routing;

namespace NSubstitute.Core;

public class ThreadLocalContext : IThreadLocalContext
{
    private static readonly IArgumentSpecification[] EmptySpecifications = [];

    private readonly RobustThreadLocal<ICallRouter?> _lastCallRouter;
    private readonly RobustThreadLocal<IList<IArgumentSpecification>> _argumentSpecifications;
    private readonly RobustThreadLocal<Func<ICall, object?[]>?> _getArgumentsForRaisingEvent;
    private readonly RobustThreadLocal<IQuery?> _currentQuery;
    private readonly RobustThreadLocal<PendingSpecInfoData> _pendingSpecificationInfo;
    private readonly RobustThreadLocal<Tuple<ICallRouter, Func<ISubstituteState, IRoute>>?> _nextRouteFactory;
    public IPendingSpecification PendingSpecification { get; }

    public ThreadLocalContext()
    {
        _lastCallRouter = new RobustThreadLocal<ICallRouter?>();
        _argumentSpecifications = new RobustThreadLocal<IList<IArgumentSpecification>>(() => []);
        _getArgumentsForRaisingEvent = new RobustThreadLocal<Func<ICall, object?[]>?>();
        _currentQuery = new RobustThreadLocal<IQuery?>();
        _pendingSpecificationInfo = new RobustThreadLocal<PendingSpecInfoData>();
        _nextRouteFactory = new RobustThreadLocal<Tuple<ICallRouter, Func<ISubstituteState, IRoute>>?>();

        PendingSpecification = new PendingSpecificationWrapper(_pendingSpecificationInfo);
    }

    public void SetLastCallRouter(ICallRouter callRouter)
    {
        _lastCallRouter.Value = callRouter;
    }

    public ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs)
    {
        var lastCallRouter = _lastCallRouter.Value;
        if (lastCallRouter == null)
            throw new CouldNotSetReturnDueToNoLastCallException();

        if (!PendingSpecification.HasPendingCallSpecInfo())
            throw new CouldNotSetReturnDueToMissingInfoAboutLastCallException();

        if (_argumentSpecifications.Value.Count > 0)
        {
            // Clear invalid arg specs so they will not affect other tests.
            _argumentSpecifications.Value.Clear();
            throw new UnexpectedArgumentMatcherException();
        }

        var pendingSpecInfo = PendingSpecification.UseCallSpecInfo()!;
        var configuredCall = lastCallRouter.LastCallShouldReturn(value, matchArgs, pendingSpecInfo);
        ClearLastCallRouter();
        return configuredCall;
    }

    public void SetNextRoute(ICallRouter callRouter, Func<ISubstituteState, IRoute> nextRouteFactory)
    {
        _nextRouteFactory.Value = Tuple.Create(callRouter, nextRouteFactory);
    }

    public Func<ISubstituteState, IRoute>? UseNextRoute(ICallRouter callRouter)
    {
        var value = _nextRouteFactory.Value;
        if (value != null && ReferenceEquals(callRouter, value.Item1))
        {
            _nextRouteFactory.Value = null;
            return value.Item2;
        }

        return null;
    }

    public void ClearLastCallRouter()
    {
        _lastCallRouter.Value = null;
    }

    public void EnqueueArgumentSpecification(IArgumentSpecification spec)
    {
        var queue = _argumentSpecifications.Value;
        if (queue == null)
            throw new SubstituteInternalException("Argument specification queue is null.");

        queue.Add(spec);
    }

    public IList<IArgumentSpecification> DequeueAllArgumentSpecifications()
    {
        var queue = _argumentSpecifications.Value;
        if (queue == null)
            throw new SubstituteInternalException("Argument specification queue is null.");

        if (queue.Count == 0)
        {
            // It's a performance optimization to avoid extra allocation and write access to ThreadLocal variable.
            // We violate public contract, as mutable list was expected as result.
            // However, in reality we never expect value to be mutated, so this optimization is fine.
            // We are not allowed to change public contract due to SemVer, so keeping that as it is.
            queue = EmptySpecifications;
        }
        else
        {
            _argumentSpecifications.Value = [];
        }

        return queue;
    }

    /// <inheritdoc/>
    public IList<IArgumentSpecification> PeekAllArgumentSpecifications()
    {
        var queue = _argumentSpecifications.Value;

        if (queue?.Count > 0)
        {
            var items = new IArgumentSpecification[queue.Count];

            queue.CopyTo(items, 0);

            return items;
        }

        return EmptySpecifications;
    }

    public void SetPendingRaisingEventArgumentsFactory(Func<ICall, object?[]> getArguments)
    {
        _getArgumentsForRaisingEvent.Value = getArguments;
    }

    public Func<ICall, object?[]>? UsePendingRaisingEventArgumentsFactory()
    {
        var result = _getArgumentsForRaisingEvent.Value;
        if (result != null)
        {
            _getArgumentsForRaisingEvent.Value = null;
        }

        return result;
    }

    public void RunInQueryContext(Action calls, IQuery query)
    {
        _currentQuery.Value = query;
        try
        {
            calls();
        }
        finally
        {
            _currentQuery.Value = null;
        }
    }

    public bool IsQuerying => _currentQuery.Value != null;

    public void RegisterInContextQuery(ICall call)
    {
        var query = _currentQuery.Value;
        if (query == null)
            throw new NotRunningAQueryException();

        query.RegisterCall(call);
    }

    private class PendingSpecificationWrapper(RobustThreadLocal<ThreadLocalContext.PendingSpecInfoData> valueHolder) : IPendingSpecification
    {
        public bool HasPendingCallSpecInfo()
        {
            return valueHolder.Value.HasValue;
        }

        public PendingSpecificationInfo? UseCallSpecInfo()
        {
            var info = valueHolder.Value;
            Clear();
            return info.ToPendingSpecificationInfo();
        }

        public void SetCallSpecification(ICallSpecification callSpecification)
        {
            valueHolder.Value = PendingSpecInfoData.FromCallSpecification(callSpecification);
        }

        public void SetLastCall(ICall lastCall)
        {
            valueHolder.Value = PendingSpecInfoData.FromLastCall(lastCall);
        }

        public void Clear()
        {
            valueHolder.Value = default;
        }
    }

    private readonly struct PendingSpecInfoData
    {
        private readonly ICallSpecification? _callSpecification;
        private readonly ICall? _lastCall;

        public bool HasValue => _lastCall != null || _callSpecification != null;

        private PendingSpecInfoData(ICallSpecification? callSpecification, ICall? lastCall)
        {
            _callSpecification = callSpecification;
            _lastCall = lastCall;
        }

        public PendingSpecificationInfo? ToPendingSpecificationInfo()
        {
            if (_callSpecification != null)
                return PendingSpecificationInfo.FromCallSpecification(_callSpecification);

            if (_lastCall != null)
                return PendingSpecificationInfo.FromLastCall(_lastCall);

            return null;
        }

        public static PendingSpecInfoData FromLastCall(ICall lastCall)
        {
            return new PendingSpecInfoData(null, lastCall);
        }

        public static PendingSpecInfoData FromCallSpecification(ICallSpecification callSpecification)
        {
            return new PendingSpecInfoData(callSpecification, null);
        }
    }
}