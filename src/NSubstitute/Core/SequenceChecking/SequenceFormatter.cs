using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;
using NSubstitute.Proxies.DelegateProxy;

namespace NSubstitute.Core.SequenceChecking
{
    public class SequenceFormatter
    {
        private readonly string _delimiter;
        private readonly CallData[] _query;
        private readonly CallData[] _actualCalls;
        private readonly bool _requiresInstanceNumbers;
        private readonly bool _hasMultipleInstances;

        public SequenceFormatter(string delimiter, CallSpecAndTarget[] querySpec, ICall[] matchingCallsInOrder)
        {
            _delimiter = delimiter;

            var instances = new InstanceTracker();
            _query = querySpec
                .Select(x => new CallData(instances.InstanceNumber(x.Target), x)).ToArray();

            _actualCalls = matchingCallsInOrder
                .Select(x => new CallData(instances.InstanceNumber(x.Target()), x)).ToArray();

            _hasMultipleInstances = instances.NumberOfInstances() > 1;
            _requiresInstanceNumbers = HasMultipleCallsOnSameType();
        }

        public string FormatQuery() { return Format(_query); }

        public string FormatActualCalls() { return Format(_actualCalls); }

        private string Format(CallData[] calls)
        {
            return calls.Select(x => x.Format(_hasMultipleInstances, _requiresInstanceNumbers)).Join(_delimiter);
        }

        private bool HasMultipleCallsOnSameType()
        {
            var lookup = new Dictionary<Type, object>();
            foreach (var x in _query)
            {
                object instance;
                if (lookup.TryGetValue(x.DeclaringType, out instance))
                {
                    if (!ReferenceEquals(x.Target, instance)) { return true; }
                }
                else
                {
                    lookup.Add(x.DeclaringType, x.Target);
                }
            }
            return false;
        }

        private class CallData
        {
            private readonly int _instanceNumber;
            private readonly ICall _call;
            private readonly CallSpecAndTarget _specAndTarget;
            private readonly CallFormatter _callFormatter = new CallFormatter();
            private readonly ArgumentFormatter _argFormatter = new ArgumentFormatter();

            public CallData(int instanceNumber, CallSpecAndTarget specAndTarget)
            {
                _instanceNumber = instanceNumber;
                _specAndTarget = specAndTarget;
            }

            public CallData(int instanceNumber, ICall call)
            {
                _instanceNumber = instanceNumber;
                _call = call;
            }

            private MethodInfo MethodInfo
            {
                get
                {
                    return _call == null
                               ? _specAndTarget.CallSpecification.GetMethodInfo()
                               : _call.GetMethodInfo();
                }
            }

            public object Target { get { return _call == null ? _specAndTarget.Target : _call.Target(); } }

            public Type DeclaringType { get { return MethodInfo.DeclaringType; } }

            public string Format(bool multipleInstances, bool includeInstanceNumber)
            {
                var call = (_call == null) 
                    ? Format(_specAndTarget) 
                    : Format(_call);

                if (!multipleInstances) { return call; }

                var instanceIdentifier = includeInstanceNumber ? _instanceNumber + "@" : "";
                var declaringType = MethodInfo.DeclaringType;
                var declaringTypeName = declaringType == typeof (DelegateCall) ? Target.ToString() : declaringType.Name;
                return string.Format("{1}{0}.{2}", declaringTypeName, instanceIdentifier, call);
            }

            private string Format(CallSpecAndTarget x)
            {
                return x.CallSpecification.ToString();
            }

            private string Format(ICall call)
            {
                var methodInfo = call.GetMethodInfo();
                var args = methodInfo.GetParameters()
                            .Zip(call.GetArguments(), (p, a) => new ArgAndParamInfo(p, a))
                            .ToArray();
                return _callFormatter.Format(methodInfo, FormatArgs(args));
            }

            private IEnumerable<string> FormatArgs(ArgAndParamInfo[] arguments)
            {
                var argsWithParamsExpanded =
                    arguments
                        .SelectMany(a => a.ParamInfo.IsParams()
                                          ? ((IEnumerable) a.Argument).Cast<object>()
                                          : ToEnumerable(a.Argument))
                        .Select(x => _argFormatter.Format(x, false))
                        .ToArray();

                return argsWithParamsExpanded;
            }

            private IEnumerable<T> ToEnumerable<T>(T value)
            {
                yield return value;
            }
        }

        private class ArgAndParamInfo
        {
            public ParameterInfo ParamInfo { get; private set; }
            public object Argument { get; private set; }

            public ArgAndParamInfo(ParameterInfo paramInfo, object argument)
            {
                ParamInfo = paramInfo;
                Argument = argument;
            }
        }
    }
}