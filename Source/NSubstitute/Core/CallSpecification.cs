using System;
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
            if (NonMatchingArgumentIndicies(call).Any()) return false;
            return true;
        }

        public string Format(ICallFormatter callFormatter)
        {
            return callFormatter.Format(_methodInfo, _argumentSpecifications, new int[0]);
        }

        public IEnumerable<int> NonMatchingArgumentIndicies(ICall call)
        {
            var arguments = call.GetArguments();
            for (var i = 0; i < arguments.Length; i++)
            {
                var argumentMatchesSpecification = ArgIsSpecifiedAndMatchesSpec(arguments[i], i);
                if (!argumentMatchesSpecification) yield return i;
            }
        }

        public ICallSpecification CreateCopyThatMatchesAnyArguments()
        {
            var anyArgs = _methodInfo.GetParameters().Select(x => new ArgumentIsAnythingSpecification(x.ParameterType)).ToArray();
            return new CallSpecification(_methodInfo, anyArgs);
        }

        public void InvokePerArgumentActions(CallInfo callInfo)
        {
            var arguments = callInfo.Args();
            var argSpecs = _argumentSpecifications;

            for (var i = 0; i < arguments.Length; i++)
            {
                argSpecs[i].Action(arguments[i]);
            }
        }

        public void PerformActionFor(ICall call)
        {
            var arguments = call.GetArguments().GetEnumerator();
            var argSpecs = _argumentSpecifications.GetEnumerator();

            while (arguments.MoveNext() && argSpecs.MoveNext())
            {
                ((IArgumentSpecification) argSpecs.Current).Action(arguments.Current);
            }
        }

        private bool HasDifferentNumberOfArguments(ICall call)
        {
            return _argumentSpecifications.Length != call.GetArguments().Length;
        }

        private bool ArgIsSpecifiedAndMatchesSpec(object argument, int argumentIndex)
        {
            if (argumentIndex >= _argumentSpecifications.Length) return false;
            return _argumentSpecifications[argumentIndex].IsSatisfiedBy(argument);
        }
    }
}