using System;
using System.Collections.Generic;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public interface IThreadLocalContext
    {
        IPendingSpecification PendingSpecification { get; }

        void SetLastCallRouter(ICallRouter callRouter);
        void ClearLastCallRouter();
        ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs);

        void EnqueueArgumentSpecification(IArgumentSpecification spec);
        IList<IArgumentSpecification> DequeueAllArgumentSpecifications();

        void SetPendingRasingEventArgumentsFactory(Func<ICall, object[]> getArguments);
        /// <summary>
        /// Returns the previously set arguments factory and resets the stored value.
        /// </summary>
        Func<ICall, object[]> UsePendingRaisingEventArgumentsFactory();

        bool IsQuerying { get; }
        /// <summary>
        /// Invokes the passed callback in a context of the specified query.
        /// </summary>
        void RunInQueryContext(Action calls, IQuery query);
        void RegisterInContextQuery(ICall call);
    }
}