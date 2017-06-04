using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Proxies.DelegateProxy
{
    public class DelegateCall : ICallRouterProvider
    {
        public static readonly MethodInfo InvokeMethodWithObjectOrVoidReturnType = typeof(DelegateCall).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        public static readonly MethodInfo InvokeMethodWithGenericReturnType = typeof(DelegateCall).GetMethod("GenericInvoke", BindingFlags.Instance | BindingFlags.NonPublic);
        readonly MethodInfo _methodToInvoke;
        readonly ICallRouter _callRouter;
        readonly Type _delegateType;
        readonly Type _returnType;
        readonly IParameterInfo[] _parameterInfos;

        public DelegateCall(ICallRouter callRouter, Type delegateType, Type returnType, IParameterInfo[] parameterInfos)
        {
            _callRouter = callRouter;
            _delegateType = delegateType;
            _returnType = returnType;
            _parameterInfos = parameterInfos;
            _methodToInvoke = GetMethodToInvoke();
        }

        private MethodInfo GetMethodToInvoke()
        {
            return ReturnsVoidType() ? InvokeMethodWithObjectOrVoidReturnType : InvokeMethodWithGenericReturnType.MakeGenericMethod(_returnType);
        }

        public ICallRouter CallRouter { get { return _callRouter; } }
        public MethodInfo MethodToInvoke { get { return _methodToInvoke; } }

        protected object Invoke(object[] arguments)
        {
            var call = new Call(MethodToInvoke, arguments, this, _parameterInfos);
            var result = _callRouter.Route(call);
            return EnsureResultCompatibleWithReturnType(result);
        }

        protected T GenericInvoke<T>(object[] arguments)
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

        bool ReturnsVoidType()
        {
            return _returnType == typeof(void);
        }

        bool ReturnsNonVoidValueType()
        {
            return !ReturnsVoidType() && _returnType.IsValueType();
        }

        object CreateDefaultForValueType(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public override string ToString()
        {
            if (!_delegateType.IsGenericType())
            {
                return _delegateType.Name;
            }
            else
            {
                var genericArgs = _delegateType.GetGenericArguments();
                var mangledName = _delegateType.Name;
                return string.Format("{0}<{1}>", 
                            new String(mangledName.TakeWhile(c => c != '`').ToArray()), 
                            string.Join(", ", genericArgs.Select(x => x.Name).ToArray()));
            }
        }
    }
}