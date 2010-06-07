using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallFormatter : ICallFormatter
    {
        public string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments)
        {
            return string.Format("{0}({1})", methodInfoOfCall.Name, FormatArgs(arguments));
        }

        private string FormatArgs(IEnumerable<object> arguments)
        {
            return string.Join(", ", arguments.Select(argument => FormatArg(argument)).ToArray());
        }

        private string FormatArg(object argument)
        {
            if (argument is string) return string.Format("\"{0}\"", argument);
            return argument.ToString();
        }
    }
}