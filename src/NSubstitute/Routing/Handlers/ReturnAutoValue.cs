using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Routing.Handlers
{
    public enum AutoValueBehaviour
    {
        UseValueForSubsequentCalls,
        ReturnAndForgetValue
    }
    public class ReturnAutoValue : ICallHandler
    {
        private readonly IEnumerable<IAutoValueProvider> _autoValueProviders;
        private readonly ICallResults _callResults;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly AutoValueBehaviour _autoValueBehaviour;

        public ReturnAutoValue(AutoValueBehaviour autoValueBehaviour, IEnumerable<IAutoValueProvider> autoValueProviders, ICallResults callResults, ICallSpecificationFactory callSpecificationFactory)
        {
            _autoValueProviders = autoValueProviders;
            _callResults = callResults;
            _callSpecificationFactory = callSpecificationFactory;
            _autoValueBehaviour = autoValueBehaviour;
        }

        public RouteAction Handle(ICall call)
        {
            if (_callResults.HasResultFor(call))
            {
                return RouteAction.Return(_callResults.GetResult(call));
            }

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
                if (_autoValueBehaviour == AutoValueBehaviour.UseValueForSubsequentCalls)
                {
                    var spec = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
                    _callResults.SetResult(spec, new ReturnValue(valueToReturn));
                }
                return RouteAction.Return(valueToReturn);
            };
        }
    }
}