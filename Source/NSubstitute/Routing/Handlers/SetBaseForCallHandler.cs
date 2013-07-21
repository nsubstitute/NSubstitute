using System;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Routing.Handlers
{
    public class SetBaseForCallHandler : ICallHandler
    {
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallResults _callResults;
        private readonly MatchArgs _matchArgs;

        public SetBaseForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallResults callResults, MatchArgs matchArgs)
        {
            _callSpecificationFactory = callSpecificationFactory;
            _callResults = callResults;
            _matchArgs = matchArgs;
        }

        public RouteAction Handle(ICall call)
        {
            var method = call.GetMethodInfo();
            if (!CanCallBase(method))
            {
                throw new CouldNotCallBaseException(method);
            }
            var callSpec = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            _callResults.SetResult(callSpec, new ReturnValueFromBase(method.ReturnType));
            return RouteAction.Continue();
        }

        private bool CanCallBase(MethodInfo methodInfo)
        {
            return methodInfo.IsVirtual && !methodInfo.IsFinal && !methodInfo.IsAbstract;
        }

        class ReturnValueFromBase : IReturn
        {
            private readonly Type _expectedType;
            public ReturnValueFromBase(Type expectedType) { _expectedType = expectedType; }
            public object ReturnFor(CallInfo info) { return info.CallBase(); }
            public Type TypeOrNull() { return _expectedType; }
            public bool CanBeAssignedTo(Type t) { return _expectedType.IsAssignableFrom(t); }
        }
    }
}