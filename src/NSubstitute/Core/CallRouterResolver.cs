using System;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallRouterResolver : ICallRouterResolver
    {
        public ICallRouter ResolveFor(object substitute)
        {
            if (substitute == null) throw new NullSubstituteReferenceException();

            if (substitute is ICallRouterProvider callRouterProvider)
            {
                return callRouterProvider.GetCallRouter();
            }

            if (substitute is Delegate delegateProxy && delegateProxy.Target is ICallRouterProvider delegateCallRouter)
            {
                return delegateCallRouter.GetCallRouter();
            }

            throw new NotASubstituteException();
        }
    }
}