using System;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateCall
    {
        internal static readonly MethodInfo DelegateCallInvoke = typeof (DelegateCall).GetMethod("Invoke");
        private readonly ICallRouter _callRouter;
        private readonly Type[] _parameterTypes;

        public DelegateCall(ICallRouter callRouter, Type[] parameterTypes)
        {
            _callRouter = callRouter;
            _parameterTypes = parameterTypes;
        }

        public object Invoke(object[] arguments)
        {
            var call = new Call(DelegateCallInvoke, arguments, this, _parameterTypes);
            return _callRouter.Route(call);
        }
    }
}