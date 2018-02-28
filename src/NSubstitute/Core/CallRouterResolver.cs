using System;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallRouterResolver : ICallRouterResolver
    {
        public ICallRouter ResolveFor(object substitute)
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            var callRouter = TryGetRouterFromObject(substitute);
            if (callRouter == null && substitute is Delegate delegateProxy)
            {
                callRouter = TryGetRouterFromObject(delegateProxy.Target);
            }

            if (callRouter == null)
                throw new NotASubstituteException();

            return callRouter;
        }

        private static ICallRouter TryGetRouterFromObject(object proxy)
        {
            switch (proxy)
            {
                case ICallRouter callRouter: return callRouter;

                case ICallRouterProvider provider: return provider.CallRouter;

                default: return null;
            }
        }
    }
}