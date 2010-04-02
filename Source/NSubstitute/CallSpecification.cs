using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NSubstitute
{
    public class CallSpecification : ICallSpecification
    {
        readonly List<IArgumentSpecification> _argumentSpecifications;
        public MethodInfo MethodInfo { get; private set; }

        public CallSpecification(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            _argumentSpecifications = new List<IArgumentSpecification>();
        }

        public bool IsSatisfiedBy(ICall call)
        {
            if (MethodInfo != call.GetMethodInfo()) return false;
            var arguments = call.GetArguments();
            if (arguments.Length != ArgumentSpecifications.Count) return false;
            for (int i = 0; i < arguments.Length; i++)
            {
                var argumentMatchesSpecification = ArgumentSpecifications[i].IsSatisfiedBy(arguments[i]);
                if (!argumentMatchesSpecification) return false;
            }
            return true;
        }

        public IList<IArgumentSpecification> ArgumentSpecifications
        {
            get { return _argumentSpecifications; }
        }
    }
}