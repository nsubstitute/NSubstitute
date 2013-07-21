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
            //Func<object> baseMethod = null;
            //if (castleInvocation.InvocationTarget != null &&
            //    castleInvocation.MethodInvocationTarget.IsAbstract != true)
            //{
            //    baseMethod = () =>
            //        {
            //            castleInvocation.Proceed();
            //            return castleInvocation.ReturnValue;
            //        };
            //}

            Func<object> baseMethod = () =>
                    {
                        castleInvocation.Proceed();
                        return castleInvocation.ReturnValue;
                    };

            return CallFactory.Create(castleInvocation.Method, castleInvocation.Arguments, castleInvocation.Proxy, baseMethod);
        }
    }
}