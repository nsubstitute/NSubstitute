using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class PropertyCallFormatter : IMethodInfoFormatter
    {
        public bool CanFormat(MethodInfo methodInfo) =>
            GetPropertyFromGetterOrSetterCall(methodInfo) != null;

        public string Format(MethodInfo methodInfo, IEnumerable<string> arguments)
        {
            var propertyInfo = GetPropertyFromGetterOrSetterCall(methodInfo)
                               ?? throw new SubstituteInternalException("The 'CanFormat' method should have guarded it.");

            var numberOfIndexParams = propertyInfo.GetIndexParameters().Length;
            var propertyName = numberOfIndexParams == 0
                ? propertyInfo.Name
                : FormatPropertyIndexer(numberOfIndexParams, arguments);

            return propertyName + FormatArgsAfterIndexParamsAsSetterArgs(numberOfIndexParams, arguments);
        }

        private PropertyInfo? GetPropertyFromGetterOrSetterCall(MethodInfo methodInfoOfCall)
        {
            return methodInfoOfCall.GetPropertyFromSetterCallOrNull() ?? methodInfoOfCall.GetPropertyFromGetterCallOrNull();
        }

        private string FormatPropertyIndexer(int numberOfIndexParameters, IEnumerable<string> arguments)
        {
            return $"this[{arguments.Take(numberOfIndexParameters).Join(", ")}]";
        }

        private bool OnlyHasIndexParameterArgs(int numberOfIndexParameters, IEnumerable<string> arguments)
        {
            return numberOfIndexParameters >= arguments.Count();
        }

        private string FormatArgsAfterIndexParamsAsSetterArgs(int numberOfIndexParameters, IEnumerable<string> arguments)
        {
            if (OnlyHasIndexParameterArgs(numberOfIndexParameters, arguments)) return string.Empty;
            return " = " + arguments.Skip(numberOfIndexParameters).Join(", ");
        }
    }
}