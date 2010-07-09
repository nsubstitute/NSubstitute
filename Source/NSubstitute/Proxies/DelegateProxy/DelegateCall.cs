using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateCall
    {
        internal static readonly MethodInfo DelegateCallInvoke = typeof (DelegateCall).GetMethod("Invoke");
        private readonly ICallRouter _callRouter;

        public DelegateCall(ICallRouter callRouter)
        {
            _callRouter = callRouter;
        }

        public object Invoke(object[] arguments)
        {
            var call = new Call(DelegateCallInvoke, arguments, this);
            return _callRouter.Route(call);
        }
    }
}