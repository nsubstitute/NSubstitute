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
        private readonly IAutoValueProvider _autoValueProvider;
        private readonly AutoValueBehaviour _autoValueBehaviour;
        private readonly IConfigureCall ConfigureCall;

        public ReturnAutoValue(AutoValueBehaviour autoValueBehaviour, IAutoValueProvider autoValueProvider, IConfigureCall configureCall)
        {
            _autoValueProvider = autoValueProvider;
            ConfigureCall = configureCall;
            _autoValueBehaviour = autoValueBehaviour;
        }

        public RouteAction Handle(ICall call)
        {
            var type = call.GetReturnType();
            var compatibleProviders = _autoValueProvider.GetValue(type);
            return compatibleProviders.Fold(
                () => NoReturnValue(call),
                x => ReturnValue(call, x));
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
                var value = _autoValueProvider.GetValue(type);

                if (value.HasValue())
                    yield return new ByRefArgument(
                        p.Index,
                        new Lazy<object>(() => value.ValueOrDefault()));
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

        private RouteAction ReturnValue(ICall call, object valueToReturn)
        {
            var callInfo = new CallInfoFactory().Create(call);
            var byRefValues = GetByRefValues(callInfo).ToArray();
            if (_autoValueBehaviour == AutoValueBehaviour.UseValueForSubsequentCalls)
            {
                ConfigureCall.SetResultForCall(call, GetReturnValue(valueToReturn, byRefValues), MatchArgs.AsSpecifiedInCall);
            }
            SetByRefValues(callInfo, byRefValues);
            return RouteAction.Return(valueToReturn);
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