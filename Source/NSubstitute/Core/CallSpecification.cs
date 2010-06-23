using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallSpecification : ICallSpecification
    {
        public MethodInfo MethodInfo { get; private set; }
        public IEnumerable<IArgumentSpecification> ArgumentSpecifications { get; private set; }

        public CallSpecification(MethodInfo methodInfo, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            ArgumentSpecifications = argumentSpecifications;
            MethodInfo = methodInfo;
        }

        public bool IsSatisfiedBy(ICall call)
        {
            if (MethodInfo != call.GetMethodInfo()) return false;
            var arguments = call.GetArguments();
            if (arguments.Length != ArgumentSpecifications.Count()) return false;
            for (int i = 0; i < arguments.Length; i++)
            {
                var argumentMatchesSpecification = ArgumentSpecifications.ElementAt(i).IsSatisfiedBy(arguments[i]);
                if (!argumentMatchesSpecification) return false;
            }
            return true;
        }

        public string Format(ICallFormatter callFormatter)
        {
            return callFormatter.Format(MethodInfo, ArgumentSpecifications);
        }
    }
}