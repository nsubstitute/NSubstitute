using System;
using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInvocationMapper
    {
        readonly static CallFactory CallFactory = new CallFactory();

        public virtual ICall Map(IInvocation castleInvocation)
        {
            Func<object> originalMethodCall = () =>
            {
                castleInvocation.Proceed();
                return castleInvocation.ReturnValue;
            };
            return CallFactory.Create(castleInvocation.Method, castleInvocation.Arguments, castleInvocation.Proxy, originalMethodCall);
        }
    }
}