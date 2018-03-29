using System;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        private readonly IThreadLocalContext _threadLocalContext;
        private readonly IRouteFactory _routeFactory;

        public CallRouterFactory(IThreadLocalContext threadLocalContext, IRouteFactory routeFactory)
        {
            _threadLocalContext = threadLocalContext ?? throw new ArgumentNullException(nameof(threadLocalContext));
            _routeFactory = routeFactory ?? throw new ArgumentNullException(nameof(routeFactory));
        }

        public ICallRouter Create(ISubstituteState substituteState)
        {
            return new CallRouter(substituteState, _threadLocalContext, _routeFactory);
        }
    }
}