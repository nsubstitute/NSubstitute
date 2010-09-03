using System;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateCall
    {
        internal static readonly MethodInfo DelegateCallInvoke = typeof (DelegateCall).GetMethod("Invoke");
        private readonly ICallRouter _callRouter;
        private readonly IParameterInfo[] _parameterInfos;

        public DelegateCall(ICallRouter callRouter, IParameterInfo[] parameterInfos)
        {
            _callRouter = callRouter;
            _parameterInfos = parameterInfos;
        }

        public object Invoke(object[] arguments)
        {
            var call = new Call(DelegateCallInvoke, arguments, this, _parameterInfos);
            return _callRouter.Route(call);
        }
    }
}