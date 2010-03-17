using System;
using System.Reflection;
using Castle.Core.Interceptor;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInvocationMapper
    {
        public virtual ICall Map(IInvocation castleInvocation)
        {
            return new CastleBasedCall(castleInvocation);            
        }

        private class CastleBasedCall : ICall {
            readonly IInvocation _castleInvocation;

            public CastleBasedCall(IInvocation castleInvocation)
            {
                _castleInvocation = castleInvocation;
            }

            public Type GetReturnType()
            {
                return GetMethodInfo().ReturnType;
            }

            public MethodInfo GetMethodInfo()
            {
                return _castleInvocation.Method;
            }

            public object[] GetArguments()
            {
                return _castleInvocation.Arguments;
            }
        }
    }
}