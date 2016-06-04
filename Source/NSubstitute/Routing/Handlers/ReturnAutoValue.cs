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
        private readonly AutoValueBehaviour _autoValueBehaviour;
        private readonly IConfigureCall ConfigureCall;

        public ReturnAutoValue(AutoValueBehaviour autoValueBehaviour, IEnumerable<IAutoValueProvider> autoValueProviders, IConfigureCall configureCall)
        {
            _autoValueProviders = autoValueProviders;
            ConfigureCall = configureCall;
            _autoValueBehaviour = autoValueBehaviour;
        }

        public RouteAction Handle(ICall call)
        {
            var type = call.GetReturnType();
            SetByRefValues(call);
            var compatibleProviders = _autoValueProviders.Where(x => x.CanProvideValueFor(type)).FirstOrNothing();
            return compatibleProviders.Fold(
                RouteAction.Continue,
                ReturnValueUsingProvider(call, type));
        }

        private void SetByRefValues(ICall call)
        {
            var args = call.GetArguments();
            var parameters =
                GetParameters(call)
                    .Select((x, i) => new {Parameter = x, Index = i})
                    .Where(x => x.Parameter.ParameterType.IsByRef);

            foreach (var p in parameters)
            {
                var type = p.Parameter.ParameterType;
                var providers =
                    _autoValueProviders
                        .Where(x => x.CanProvideValueFor(type))
                        .FirstOrNothing();

                if (providers.HasValue())
                    args[p.Index] = providers.ValueOrDefault().GetValue(type);
            }
        }

        private static IParameterInfo[] GetParameters(ICall call)
        {
            return call.GetParameterInfos() ?? new IParameterInfo[0];
        }

        private Func<IAutoValueProvider, RouteAction> ReturnValueUsingProvider(ICall call, Type type)
        {
            return provider =>
            {
                var valueToReturn = provider.GetValue(type);
                if (_autoValueBehaviour == AutoValueBehaviour.UseValueForSubsequentCalls)
                {
                    ConfigureCall.SetResultForCall(call, new ReturnValue(valueToReturn), MatchArgs.AsSpecifiedInCall);
                }
                return RouteAction.Return(valueToReturn);
            };
        }
    }
}