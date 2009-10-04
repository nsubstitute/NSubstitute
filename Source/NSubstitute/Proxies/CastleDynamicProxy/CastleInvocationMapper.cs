using System;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInvocationMapper
    {
        public virtual IInvocation Map(Castle.Core.Interceptor.IInvocation castleInvocation)
        {
            return new CastleBasedInvocation(castleInvocation);            
        }

        private class CastleBasedInvocation : IInvocation {
            readonly Castle.Core.Interceptor.IInvocation _castleInvocation;

            public CastleBasedInvocation(Castle.Core.Interceptor.IInvocation castleInvocation)
            {
                _castleInvocation = castleInvocation;
            }

            public Type GetReturnType()
            {
                return _castleInvocation.Method.ReturnType;
            }
        }
    }
}