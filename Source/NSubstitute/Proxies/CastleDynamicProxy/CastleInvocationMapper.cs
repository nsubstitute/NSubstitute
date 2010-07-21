using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInvocationMapper
    {
        public virtual ICall Map(IInvocation castleInvocation)
        {
            return new Call(castleInvocation.Method, castleInvocation.Arguments, castleInvocation.Proxy);
        }
    }
}