using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnNewSubstituteForTypeHandler : ICallHandler
    {
        private readonly ISubstituteFactory _substituteFactory;
        private readonly IResultSetter _resultSetter;

        public ReturnNewSubstituteForTypeHandler(ISubstituteFactory substituteFactory, IResultSetter resultSetter)
        {
            _substituteFactory = substituteFactory;
            _resultSetter = resultSetter;
        }

        public RouteAction Handle(ICall call)
        {
            if (!call.GetReturnType().IsInterface) return RouteAction.Continue();

            var newSubstitute = _substituteFactory.Create(new[] { call.GetReturnType() }, new object[0]);
            _resultSetter.SetResultForCall(call, new ReturnValue(newSubstitute), MatchArgs.AsSpecifiedInCall);
            return RouteAction.Return(newSubstitute);
        }
    }
}