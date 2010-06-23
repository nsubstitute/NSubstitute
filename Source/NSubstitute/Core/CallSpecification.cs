using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallSpecification : ICallSpecification
    {
        readonly IEnumerable<IArgumentSpecification> _argumentSpecifications;
        readonly MethodInfo _methodInfo;

        public CallSpecification(MethodInfo methodInfo, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _argumentSpecifications = argumentSpecifications;
            _methodInfo = methodInfo;
        }

        public bool IsSatisfiedBy(ICall call)
        {
            if (_methodInfo != call.GetMethodInfo()) return false;
            var arguments = call.GetArguments();
            if (arguments.Length != _argumentSpecifications.Count()) return false;
            for (int i = 0; i < arguments.Length; i++)
            {
                var argumentMatchesSpecification = _argumentSpecifications.ElementAt(i).IsSatisfiedBy(arguments[i]);
                if (!argumentMatchesSpecification) return false;
            }
            return true;
        }

        public string Format(ICallFormatter callFormatter)
        {
            return callFormatter.Format(_methodInfo, _argumentSpecifications);
        }
    }
}