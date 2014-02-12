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
            var compatibleProviders = _autoValueProviders.Where(x => x.CanProvideValueFor(type)).FirstOrNothing();
            return compatibleProviders.Fold(
                RouteAction.Continue,
                ReturnValueUsingProvider(call, type));
        }

        private Func<IAutoValueProvider, RouteAction> ReturnValueUsingProvider(ICall call, Type type)
        {
            return provider =>
            {
                var valueToReturn = provider.GetValue(type);
                ConfigureCall.SetResultForCall(call, new ReturnValue(valueToReturn), MatchArgs.AsSpecifiedInCall);
                return RouteAction.Return(valueToReturn);
            };
        }
    }
}