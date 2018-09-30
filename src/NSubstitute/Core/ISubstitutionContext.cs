using System;
using System.Collections.Generic;
using NSubstitute.Core.Arguments;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public interface ISubstitutionContext
    {
        ISubstituteFactory SubstituteFactory { get; }
        IRouteFactory RouteFactory { get; }
        ICallSpecificationFactory CallSpecificationFactory { get; }

        /// <summary>
        /// A thread bound state of the NSubstitute context. Usually this API is used to provide the fluent
        /// features of the NSubstitute.
        /// </summary>
        IThreadLocalContext ThreadContext { get; }

        ICallRouter GetCallRouterFor(object substitute);

        [Obsolete("This property is obsolete and will be removed in a future version of the product.")]
        SequenceNumberGenerator SequenceNumberGenerator { get; }

        [Obsolete("This property is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.PendingSpecification) + " property instead.")]
        PendingSpecificationInfo PendingSpecificationInfo { get; set; }

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.LastCallShouldReturn) + "() method instead.")]
        ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.SetLastCallRouter) + "() method instead.")]
        void LastCallRouter(ICallRouter callRouter);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.EnqueueArgumentSpecification) + "() method instead.")]
        void EnqueueArgumentSpecification(IArgumentSpecification spec);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.DequeueAllArgumentSpecifications) + "() method instead.")]
        IList<IArgumentSpecification> DequeueAllArgumentSpecifications();

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.SetPendingRasingEventArgumentsFactory) + "() method instead.")]
        void RaiseEventForNextCall(Func<ICall, object[]> getArguments);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.UsePendingRaisingEventArgumentsFactory) + "() method instead.")]
        Func<ICall, object[]> DequeuePendingRaisingEventArguments();

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.RunInQueryContext) + "() method instead.")]
        IQueryResults RunQuery(Action calls);

        [Obsolete("This property is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.IsQuerying) + " property instead.")]
        bool IsQuerying { get; }

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.RegisterInContextQuery) + "() method instead.",
                  error: true)]
        void AddToQuery(object target, ICallSpecification callSpecification);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.ClearLastCallRouter) + "() method instead.")]
        void ClearLastCallRouter();

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(RouteFactory) + " property instead.")]
        IRouteFactory GetRouteFactory();
    }
}