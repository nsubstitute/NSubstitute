using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnAutoValueForThisAndSubsequentCallsHandler : ICallHandler
    {
        private readonly IEnumerable<IAutoValueProvider> _autoValueProviders;
        private readonly IResultSetter _resultSetter;

        public ReturnAutoValueForThisAndSubsequentCallsHandler(IEnumerable<IAutoValueProvider> autoValueProviders, IResultSetter resultSetter)
        {
            _autoValueProviders = autoValueProviders;
            _resultSetter = resultSetter;
        }

        public RouteAction Handle(ICall call)
        {
            var type = call.GetReturnType();
            var compatibleProviders = _autoValueProviders.Where(x => x.CanProvideValueFor(type));
            if (compatibleProviders.Any())
            {
                    var valueToReturn = compatibleProviders.First().GetValue(type);
                    _resultSetter.SetResultForCall(call, new ReturnValue(valueToReturn), MatchArgs.AsSpecifiedInCall);
                    return RouteAction.Return(valueToReturn);
            }
            return RouteAction.Continue();
        }
    }
}