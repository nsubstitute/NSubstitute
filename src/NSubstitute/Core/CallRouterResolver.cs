using System;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallRouterResolver : ICallRouterResolver
    {
        public ICallRouter ResolveFor(object substitute)
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            if (substitute is ICallRouter callRouter)
            {
                return callRouter;
            }

            if (substitute is Delegate delegateProxy && delegateProxy.Target is ICallRouter delegateCallRouter)
            {
                return delegateCallRouter;
            }

            throw new NotASubstituteException();
        }
    }
}