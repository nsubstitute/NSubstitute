using System;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateCall
    {
        internal static readonly MethodInfo DelegateCallInvoke = typeof (DelegateCall).GetMethod("Invoke");
        private readonly ICallRouter _callRouter;
        readonly Type _returnType;
        private readonly IParameterInfo[] _parameterInfos;

        public DelegateCall(ICallRouter callRouter, Type returnType, IParameterInfo[] parameterInfos)
        {
            _callRouter = callRouter;
            _returnType = returnType;
            _parameterInfos = parameterInfos;
        }

        public object Invoke(object[] arguments)
        {
            var call = new Call(DelegateCallInvoke, arguments, this, _parameterInfos);
            var result = _callRouter.Route(call);
            return EnsureResultCompatibleWithReturnType(result);
        }

        object EnsureResultCompatibleWithReturnType(object result)
        {
            if (ReturnsNonVoidValueType() && result == null)
            {
                return CreateDefaultForValueType(_returnType);
            }
            return result;
        }

        bool ReturnsNonVoidValueType()
        {
            return _returnType != typeof(void) && _returnType.IsValueType;
        }

        object CreateDefaultForValueType(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}