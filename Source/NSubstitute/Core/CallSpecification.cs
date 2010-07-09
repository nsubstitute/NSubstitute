using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallSpecification : ICallSpecification
    {
        readonly IArgumentSpecification[] _argumentSpecifications;
        readonly MethodInfo _methodInfo;

        public CallSpecification(MethodInfo methodInfo, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _argumentSpecifications = argumentSpecifications.ToArray();
            _methodInfo = methodInfo;
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
            return callFormatter.Format(_methodInfo, _argumentSpecifications);
        }

        private bool HasDifferentNumberOfArguments(ICall call)
        {
            return _argumentSpecifications.Length != call.GetArguments().Length;
        }

        public IEnumerable<int> NonMatchingArgumentIndicies(ICall call)
        {
            var arguments = call.GetArguments();
            for (var i = 0; i < arguments.Length; i++)
            {
                if (i >= _argumentSpecifications.Length) yield return i;

                var argumentMatchesSpecification = _argumentSpecifications[i].IsSatisfiedBy(arguments[i]);
                if (!argumentMatchesSpecification) yield return i; 
            }
        }
    }
}