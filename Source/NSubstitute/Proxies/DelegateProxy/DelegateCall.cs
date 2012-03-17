using System;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateCall
    {
        private readonly MethodInfo _delegateCallInvoke;
        private readonly ICallRouter _callRouter;
        readonly Type _returnType;
        private readonly IParameterInfo[] _parameterInfos;

        public DelegateCall(ICallRouter callRouter, Type returnType, IParameterInfo[] parameterInfos)
        {
            _callRouter = callRouter;
            _returnType = returnType;
            _parameterInfos = parameterInfos;
            _delegateCallInvoke = CreateDelegateCallInvoke();
        }

        private MethodInfo CreateDelegateCallInvoke()
        {
            if (ReturnsVoidType())
            {
                return CreateObjectDelegateCallInvoke();
            }

            return CreateGenericDelegateCallInvoke();
        }

        public MethodInfo DelegateCallInvoke
        {
            get { return _delegateCallInvoke; }
        }

        private MethodInfo CreateObjectDelegateCallInvoke()
        {
            return typeof (DelegateCall).GetMethod("Invoke");
        }

        private MethodInfo CreateGenericDelegateCallInvoke()
        {
            MethodInfo genericInvokeMethodInfo = typeof (DelegateCall).GetMethod("GenericInvoke");
            return genericInvokeMethodInfo.MakeGenericMethod(_returnType);
        }

        public object Invoke(object[] arguments)
        {
            var call = new Call(DelegateCallInvoke, arguments, this, _parameterInfos);
            var result = _callRouter.Route(call);

            return EnsureResultCompatibleWithReturnType(result);
        }

        public T GenericInvoke<T>(object[] arguments)
        {
            return (T)Invoke(arguments);
        }

        object EnsureResultCompatibleWithReturnType(object result)
        {
            if (ReturnsNonVoidValueType() && result == null)
            {
                return CreateDefaultForValueType(_returnType);
            }
            return result;
        }

        private bool ReturnsVoidType()
        {
            return _returnType == typeof (void);
        }

        bool ReturnsNonVoidValueType()
        {
            return !ReturnsVoidType() && _returnType.IsValueType;
        }

        object CreateDefaultForValueType(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}