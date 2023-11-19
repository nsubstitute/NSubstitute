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
            var args = string.Join(", ", arguments);
            return $"{methodInfo.Name}{FormatGenericType(methodInfo)}({args})";
        }

        private static string FormatGenericType(MethodInfo methodInfoOfCall)
        {
            if (!methodInfoOfCall.IsGenericMethod)
            {
                return string.Empty;
            }

            var genericArgs = methodInfoOfCall.GetGenericArguments();
            return $"<{string.Join(", ", genericArgs.Select(x => x.GetNonMangledTypeName()))}>";
        }
    }
}