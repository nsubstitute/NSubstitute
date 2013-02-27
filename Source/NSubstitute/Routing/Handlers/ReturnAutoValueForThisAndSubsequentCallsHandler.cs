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
        private readonly IConfigureCall ConfigureCall;

        public ReturnAutoValueForThisAndSubsequentCallsHandler(IEnumerable<IAutoValueProvider> autoValueProviders, IConfigureCall configureCall)
        {
            _autoValueProviders = autoValueProviders;
            ConfigureCall = configureCall;
        }

        public RouteAction Handle(ICall call)
        {
            var type = call.GetReturnType();
            var compatibleProviders = _autoValueProviders.Where(x => x.CanProvideValueFor(type));
            if (compatibleProviders.Any())
            {
                    var valueToReturn = compatibleProviders.First().GetValue(type);
                    ConfigureCall.SetResultForCall(call, new ReturnValue(valueToReturn), MatchArgs.AsSpecifiedInCall);
                    return RouteAction.Return(valueToReturn);
            }
            return RouteAction.Continue();
        }
    }
}