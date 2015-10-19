using System;
#if NET4
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallRouterResolver : ICallRouterResolver
    {
#if NET4
        IDictionary<object, ICallRouter> _callRouterMappings = new ConcurrentDictionary<object, ICallRouter>();
#else
        IDictionary<object, ICallRouter> _callRouterMappings = new Dictionary<object, ICallRouter>();
#endif

        public ICallRouter ResolveFor(object substitute)
        {
            if (substitute == null) throw new NullSubstituteReferenceException();
            if (substitute is ICallRouter) return (ICallRouter)substitute;
            if (substitute is ICallRouterProvider) return ((ICallRouterProvider) substitute).CallRouter;
            ICallRouter callRouter;
#if NET4
            if (_callRouterMappings.TryGetValue(substitute, out callRouter))
            {
                return callRouter;
            }
#else
            lock (_callRouterMappings)
            {
                if (_callRouterMappings.TryGetValue(substitute, out callRouter))
                {
                    return callRouter;
                }
            }
#endif
            throw new NotASubstituteException();
        }

        public void Register(object proxy, ICallRouter callRouter)
        {
            if (proxy is ICallRouter) return;
            if (proxy is ICallRouterProvider) return;

#if NET4
            _callRouterMappings.Add(proxy, callRouter);
#else
            lock (_callRouterMappings)
            {
                _callRouterMappings.Add(proxy, callRouter);
            }
#endif
        }
    }
}