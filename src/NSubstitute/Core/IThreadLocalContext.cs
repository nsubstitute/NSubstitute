using System;
using System.Collections.Generic;
using NSubstitute.Core.Arguments;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public interface IThreadLocalContext
    {
        IPendingSpecification PendingSpecification { get; }

        void SetLastCallRouter(ICallRouter callRouter);
        void ClearLastCallRouter();
        ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs);

        /// <summary>
        /// Sets the route to use for the next call dispatch on the current thread for the specified <paramref name="callRouter"/>.
        /// </summary>
        void SetNextRoute(ICallRouter callRouter, Func<ISubstituteState, IRoute> nextRouteFactory);
        /// <summary>
        /// Returns the previously configured next route and resets the stored value.
        /// If route was configured for the different router, returns <see langword="null"/> and persist the route info.
        /// </summary>
        Func<ISubstituteState, IRoute>? UseNextRoute(ICallRouter callRouter);

        void EnqueueArgumentSpecification(IArgumentSpecification spec);
        IList<IArgumentSpecification> DequeueAllArgumentSpecifications();

        void SetPendingRaisingEventArgumentsFactory(Func<ICall, object?[]> getArguments);
        /// <summary>
        /// Returns the previously set arguments factory and resets the stored value.
        /// </summary>
        Func<ICall, object?[]>? UsePendingRaisingEventArgumentsFactory();

        bool IsQuerying { get; }
        /// <summary>
        /// Invokes the passed callback in a context of the specified query.
        /// </summary>
        void RunInQueryContext(Action calls, IQuery query);
        void RegisterInContextQuery(ICall call);
    }
}