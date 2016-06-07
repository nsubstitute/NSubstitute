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
            var compatibleProviders = _autoValueProviders.Where(x => x.CanProvideValueFor(type)).FirstOrNothing();
            return compatibleProviders.Fold(
                () => NoReturnValue(call),
                ReturnValueUsingProvider(call, type));
        }

        private IEnumerable<ByRefArgument> GetByRefValues(CallInfo args)
        {
            var parameters =
                args.ArgTypes()
                    .Select((x, i) => new { Parameter = x, Index = i })
                    .Where(x => x.Parameter.IsByRef);

            foreach (var p in parameters)
            {
                var type = p.Parameter;
                var providers =
                    _autoValueProviders
                        .Where(x => x.CanProvideValueFor(type))
                        .FirstOrNothing();

                if (providers.HasValue())
                    yield return new ByRefArgument(
                        p.Index,
                        new Lazy<object>(() => providers.ValueOrDefault().GetValue(type)));
            }
        }

        private void SetByRefValues(CallInfo callInfo, IEnumerable<ByRefArgument> values)
        {
            foreach (var value in values)
                callInfo[value.Index] = value.Value.Value;
        }

        private ReturnValueFromFunc<T> GetReturnValue<T>(T value, IEnumerable<ByRefArgument> byRefValues)
        {
            return new ReturnValueFromFunc<T>(callInfo =>
            {
                SetByRefValues(callInfo, byRefValues);
                return value;
            });
        }

        private RouteAction NoReturnValue(ICall call)
        {
            var callInfo = new CallInfoFactory().Create(call);
            var byRefValues = GetByRefValues(callInfo).ToArray();

            if (byRefValues.Length == 0)
                return RouteAction.Continue();

            if (_autoValueBehaviour == AutoValueBehaviour.UseValueForSubsequentCalls)
                ConfigureCall.SetResultForCall(call,
                    GetReturnValue<object>(null, byRefValues),
                    MatchArgs.AsSpecifiedInCall);

            SetByRefValues(callInfo, byRefValues);
            return RouteAction.Continue();
        }

        private Func<IAutoValueProvider, RouteAction> ReturnValueUsingProvider(ICall call, Type type)
        {
            return provider =>
            {
                var valueToReturn = provider.GetValue(type);
                var callInfo = new CallInfoFactory().Create(call);
                var byRefValues = GetByRefValues(callInfo).ToArray();
                if (_autoValueBehaviour == AutoValueBehaviour.UseValueForSubsequentCalls)
                {
                    ConfigureCall.SetResultForCall(call, GetReturnValue(valueToReturn, byRefValues), MatchArgs.AsSpecifiedInCall);
                }
                SetByRefValues(callInfo, byRefValues);
                return RouteAction.Return(valueToReturn);
            };
        }

        private class ByRefArgument
        {
            private readonly int _index;
            private readonly Lazy<object> _value;

            public ByRefArgument(int index, Lazy<object> value)
            {
                _index = index;
                _value = value;
            }

            public int Index
            {
                get { return _index; }
            }

            public Lazy<object> Value
            {
                get { return _value; }
            }
        }
    }
}