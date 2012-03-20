using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class MethodFormatter : IMethodInfoFormatter
    {
        public bool CanFormat(MethodInfo methodInfo)
        {
            return true;
        }

        public string Format(MethodInfo methodInfo, IEnumerable<string> arguments)
        {
            return string.Format("{0}{1}({2})", methodInfo.Name, FormatGenericType(methodInfo), arguments.Join(", "));
        }

        private string FormatGenericType(MethodInfo methodInfoOfCall)
        {
            if (!methodInfoOfCall.IsGenericMethod) return string.Empty;
            var genericArgs = methodInfoOfCall.GetGenericArguments();
            return "<" + string.Join(", ", genericArgs.Select(x => x.Name).ToArray()) + ">";
        }
    }
}