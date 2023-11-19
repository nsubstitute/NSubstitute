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
        private readonly IAutoValueProvider[] _autoValueProviders;
        private readonly ICallResults _callResults;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly AutoValueBehaviour _autoValueBehaviour;

        public ReturnAutoValue(AutoValueBehaviour autoValueBehaviour, IEnumerable<IAutoValueProvider> autoValueProviders, ICallResults callResults, ICallSpecificationFactory callSpecificationFactory)
        {
            _autoValueProviders = autoValueProviders.AsArray();
            _callResults = callResults;
            _callSpecificationFactory = callSpecificationFactory;
            _autoValueBehaviour = autoValueBehaviour;
        }

        public RouteAction Handle(ICall call)
        {
            if (_callResults.TryGetResult(call, out var cachedResult))
            {
                return RouteAction.Return(cachedResult);
            }

            var type = call.GetReturnType();

            // This is a hot method which is invoked frequently and has major impact on performance.
            // Therefore, the LINQ cycle was unwinded to loop.
            foreach (var autoValueProvider in _autoValueProviders)
            {
                if (autoValueProvider.CanProvideValueFor(type))
                {
                    return RouteAction.Return(GetResultValueUsingProvider(call, type, autoValueProvider));
                }
            }

            return RouteAction.Continue();
        }

        private object? GetResultValueUsingProvider(ICall call, Type type, IAutoValueProvider provider)
        {
            var valueToReturn = provider.GetValue(type);
            if (_autoValueBehaviour == AutoValueBehaviour.UseValueForSubsequentCalls)
            {
                var spec = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
                _callResults.SetResult(spec, new ReturnValue(valueToReturn));
            }

            return valueToReturn;
        }
    }
}