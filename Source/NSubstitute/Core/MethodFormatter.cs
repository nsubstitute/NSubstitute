using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class MethodFormatter : IMethodInfoFormatter
    {
        IArgumentsFormatter _argumentsFormatter;

        public MethodFormatter(IArgumentsFormatter argumentsFormatter)
        {
            _argumentsFormatter = argumentsFormatter;
        }

        public bool CanFormat(MethodInfo methodInfo)
        {
            return true;
        }

        public string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            string genericInfo = null;
            if (methodInfoOfCall.IsGenericMethod)
            {
                var genericArgs = methodInfoOfCall.GetGenericArguments();
                genericInfo = "<" + string.Join(", ", genericArgs.Select(x => x.Name).ToArray()) + ">";
            }
            return string.Format("{0}{1}({2})", methodInfoOfCall.Name, genericInfo, _argumentsFormatter.Format(arguments, argumentsToHighlight));
        }
    }
}