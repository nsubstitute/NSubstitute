using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallFormatter : ICallFormatter
    {
        public string Format(MethodInfo methodInfoOfCall, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            return string.Format("{0}({1})", methodInfoOfCall.Name, FormatArgs(argumentSpecifications));
        }

        private string FormatArgs(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            return string.Join(", ", argumentSpecifications.Select(argument => FormatArg(argument)).ToArray());
        }

        private string FormatArg(object argument)
        {
            if (argument is string) return string.Format("\"{0}\"", argument);
            return argument.ToString();
        }
    }
}