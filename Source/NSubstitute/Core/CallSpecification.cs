using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallSpecification : ICallSpecification
    {
        readonly MethodInfo _methodInfo;
        readonly IArgumentSpecification[] _argumentSpecifications;

        public CallSpecification(MethodInfo methodInfo, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _methodInfo = methodInfo;
            _argumentSpecifications = argumentSpecifications.ToArray();
        }

        public bool IsSatisfiedBy(ICall call)
        {
            if (_methodInfo != call.GetMethodInfo()) return false;
            if (HasDifferentNumberOfArguments(call)) return false;
            if (NonMatchingArguments(call).Any()) return false;
            return true;
        }

        public string Format(ICallFormatter callFormatter)
        {
            return callFormatter.Format(_methodInfo, _argumentSpecifications, new ArgumentMatchInfo[0]);
        }

        public IEnumerable<ArgumentMatchInfo> NonMatchingArguments(ICall call)
        {
            var arguments = call.GetArguments();
            return arguments
                    .Select((arg, index) => new ArgumentMatchInfo(index, arg, _argumentSpecifications[index]))
                    .Where(x => !x.IsMatch);
        }

        public ICallSpecification CreateCopyThatMatchesAnyArguments()
        {
            var anyArgs = _methodInfo
                .GetParameters()
                .Zip(
                    _argumentSpecifications,
                    (p, spec) => spec.CreateCopyMatchingAnyArgOfType(p.ParameterType)
                )
                .ToArray();
            return new CallSpecification(_methodInfo, anyArgs);
        }

        public void InvokePerArgumentActions(CallInfo callInfo)
        {
            var arguments = callInfo.Args();
            var argSpecs = _argumentSpecifications;

            for (var i = 0; i < arguments.Length; i++)
            {
                argSpecs[i].RunAction(arguments[i]);
            }
        }

        private bool HasDifferentNumberOfArguments(ICall call)
        {
            return _argumentSpecifications.Length != call.GetArguments().Length;
        }
    }
}