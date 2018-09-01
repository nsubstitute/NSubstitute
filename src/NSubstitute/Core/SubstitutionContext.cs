using System;
using System.Collections.Generic;
using NSubstitute.Core.Arguments;
using NSubstitute.Core.DependencyInjection;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        public static ISubstitutionContext Current { get; set; }

        private readonly ICallRouterResolver _callRouterResolver;
        public ISubstituteFactory SubstituteFactory { get; }
        public IRouteFactory RouteFactory { get; }
        public IThreadLocalContext ThreadContext { get; }
        public ICallSpecificationFactory CallSpecificationFactory { get; }

        static SubstitutionContext()
        {
            Current = NSubstituteDefaultFactory.CreateSubstitutionContext();
        }

        public SubstitutionContext(ISubstituteFactory substituteFactory,
            IRouteFactory routeFactory,
            ICallSpecificationFactory callSpecificationFactory,
            IThreadLocalContext threadLocalContext,
            ICallRouterResolver callRouterResolver)
        {
            SubstituteFactory = substituteFactory ?? throw new ArgumentNullException(nameof(substituteFactory));
            RouteFactory = routeFactory ?? throw new ArgumentNullException(nameof(routeFactory));
            CallSpecificationFactory = callSpecificationFactory ?? throw new ArgumentNullException(nameof(callSpecificationFactory));
            ThreadContext = threadLocalContext ?? throw new ArgumentNullException(nameof(threadLocalContext));
            _callRouterResolver = callRouterResolver ?? throw new ArgumentNullException(nameof(callRouterResolver));

#pragma warning disable 618 // Obsolete
            SequenceNumberGenerator = NSubstituteDefaultFactory.DefaultContainer.Resolve<SequenceNumberGenerator>();
#pragma warning restore 618 // Obsolete
        }

        public ICallRouter GetCallRouterFor(object substitute) =>
            _callRouterResolver.ResolveFor(substitute);


        // ***********************************************************
        // ********************** OBSOLETE API **********************
        // API below is obsolete and present for the binary compatibility with the previous versions.
        // All implementations are relaying to the non-obsolete members.

        [Obsolete("This property is obsolete and will be removed in a future version of the product.")]
        public SequenceNumberGenerator SequenceNumberGenerator { get; }
 
        [Obsolete("This property is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.IsQuerying) + " property instead.")]
        public bool IsQuerying => ThreadContext.IsQuerying;

        [Obsolete("This property is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.PendingSpecification) + " property instead.")]
        public PendingSpecificationInfo PendingSpecificationInfo
        {
            get
            {
                if (!ThreadContext.PendingSpecification.HasPendingCallSpecInfo())
                    return null;

                // This removes the pending specification, so we need to restore it back.
                var consumedSpecInfo = ThreadContext.PendingSpecification.UseCallSpecInfo();
                PendingSpecificationInfo = consumedSpecInfo;

                return consumedSpecInfo;
            }
            set
            {
                if (value == null)
                {
                    ThreadContext.PendingSpecification.Clear();
                    return;
                }

                // Emulate the old API. A bit clumsy, however it's here for the backward compatibility only
                // and is not expected to be used frequently.
                var unwrappedValue = value.Handle(
                    spec => Tuple.Create(spec, (ICall) null),
                    call => Tuple.Create((ICallSpecification) null, call));

                if (unwrappedValue.Item1 != null)
                {
                    ThreadContext.PendingSpecification.SetCallSpecification(unwrappedValue.Item1);
                }
                else
                {
                    ThreadContext.PendingSpecification.SetLastCall(unwrappedValue.Item2);
                }
            }
        }

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.LastCallShouldReturn) + "() method instead.")]
        public ConfiguredCall LastCallShouldReturn(IReturn value, MatchArgs matchArgs) =>
            ThreadContext.LastCallShouldReturn(value, matchArgs);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.ClearLastCallRouter) + "() method instead.")]
        public void ClearLastCallRouter() =>
            ThreadContext.ClearLastCallRouter();

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(RouteFactory) + " property instead.")]
        public IRouteFactory GetRouteFactory() =>
            RouteFactory;

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.SetLastCallRouter) + "() method instead.")]
        public void LastCallRouter(ICallRouter callRouter) =>
            ThreadContext.SetLastCallRouter(callRouter);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.EnqueueArgumentSpecification) + "() method instead.")]
        public void EnqueueArgumentSpecification(IArgumentSpecification spec) =>
            ThreadContext.EnqueueArgumentSpecification(spec);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.DequeueAllArgumentSpecifications) + "() method instead.")]
        public IList<IArgumentSpecification> DequeueAllArgumentSpecifications() =>
            ThreadContext.DequeueAllArgumentSpecifications();

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.SetPendingRasingEventArgumentsFactory) + "() method instead.")]
        public void RaiseEventForNextCall(Func<ICall, object[]> getArguments) =>
            ThreadContext.SetPendingRasingEventArgumentsFactory(getArguments);

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.UsePendingRaisingEventArgumentsFactory) + "() method instead.")]
        public Func<ICall, object[]> DequeuePendingRaisingEventArguments() =>
            ThreadContext.UsePendingRaisingEventArgumentsFactory();

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.RegisterInContextQuery) + "() method instead.",
                  error: true)]
        public void AddToQuery(object target, ICallSpecification callSpecification)
        {
            // We cannot simulate the API anymore as it changed drastically.
            // That should be fine, as this method was expected to be called from the NSubstitute core only.
            throw new NotSupportedException(
                "This API was obsolete and is not supported anymore. " +
                "Please Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.RegisterInContextQuery) + "() method instead.");
        }

        [Obsolete("This method is obsolete and will be removed in a future version of the product. " +
                  "Use the " + nameof(ThreadContext) + "." + nameof(IThreadLocalContext.RunInQueryContext) + "() method instead.")]
        public IQueryResults RunQuery(Action calls)
        {
            var query = new Query(CallSpecificationFactory);
            ThreadContext.RunInQueryContext(calls, query);
            return query.Result();
        }
    }
}