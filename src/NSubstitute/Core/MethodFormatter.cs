using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Proxies.DelegateProxy;

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

            return IsDelegateProxy(methodInfo)
                ? string.Format("Invoke({0})", args)
                : string.Format("{0}{1}({2})", methodInfo.Name, FormatGenericType(methodInfo), args);
        }

        private string FormatGenericType(MethodInfo methodInfoOfCall)
        {
            if (!methodInfoOfCall.IsGenericMethod) return string.Empty;
            var genericArgs = methodInfoOfCall.GetGenericArguments();
            return "<" + string.Join(", ", genericArgs.Select(x => x.GetNonMangledTypeName()).ToArray()) + ">";
        }

        private static bool IsDelegateProxy(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<ProxiedDelegateTypeAttribute>() != null;
        }
    }
}