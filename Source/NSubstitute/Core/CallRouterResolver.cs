using System.Collections.Concurrent;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallRouterResolver : ICallRouterResolver
    {
        readonly ConcurrentDictionary<object, ICallRouter> _callRouterMappings = new ConcurrentDictionary<object, ICallRouter>();

        public ICallRouter ResolveFor(object substitute)
        {
            if (substitute == null) throw new NullSubstituteReferenceException();
            if (substitute is ICallRouter) return (ICallRouter)substitute;
            if (substitute is ICallRouterProvider) return ((ICallRouterProvider) substitute).CallRouter;
            ICallRouter callRouter;
            if (_callRouterMappings.TryGetValue(substitute, out callRouter))
            {
                return callRouter;
            }
            throw new NotASubstituteException();
        }

        public void Register(object proxy, ICallRouter callRouter)
        {
            if (proxy is ICallRouter) return;
            if (proxy is ICallRouterProvider) return;

            _callRouterMappings.AddOrUpdate(proxy, callRouter, (o,c) => callRouter);
        }
    }
}