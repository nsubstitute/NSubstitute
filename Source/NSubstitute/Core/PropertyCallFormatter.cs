using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class PropertyCallFormatter : IMethodInfoFormatter
    {
        IArgumentsFormatter _argumentsFormatter;

        public PropertyCallFormatter(IArgumentsFormatter argumentsFormatter)
        {
            _argumentsFormatter = argumentsFormatter;
        }

        public bool CanFormat(MethodInfo methodInfo)
        {
            return GetPropertyFromGetterOrSetterCall(methodInfo) != null;
        }

        public string Format(MethodInfo methodInfo, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            var propertyInfo = GetPropertyFromGetterOrSetterCall(methodInfo);
            var numberOfIndexParams = propertyInfo.GetIndexParameters().Length;
            var propertyName =
                (numberOfIndexParams == 0)
                    ? propertyInfo.Name
                    : FormatPropertyIndexer(numberOfIndexParams, argumentsToHighlight, arguments);

            return propertyName + FormatArgsAfterIndexParamsAsSetterArgs(numberOfIndexParams, argumentsToHighlight, arguments);
        }

        private PropertyInfo GetPropertyFromGetterOrSetterCall(MethodInfo methodInfoOfCall)
        {
            return methodInfoOfCall.GetPropertyFromSetterCallOrNull() ?? methodInfoOfCall.GetPropertyFromGetterCallOrNull();
        }

        private string FormatPropertyIndexer(int numberofIndexParameters, IEnumerable<int> argumentsToHighlight, IEnumerable<object> arguments)
        {
            return "this[" + _argumentsFormatter.Format(arguments.Take(numberofIndexParameters), argumentsToHighlight) + "]";
        }

        private bool OnlyHasIndexParameterArgs(int numberOfIndexParameters, IEnumerable<object> arguments)
        {
            return numberOfIndexParameters >= arguments.Count();
        }

        private string FormatArgsAfterIndexParamsAsSetterArgs(int numberOfIndexParameters, IEnumerable<int> argumentsToHighlight, IEnumerable<object> arguments)
        {
            if (OnlyHasIndexParameterArgs(numberOfIndexParameters, arguments)) return string.Empty;
            return " = " + _argumentsFormatter.Format(arguments.Skip(numberOfIndexParameters), argumentsToHighlight.Select(x => x - numberOfIndexParameters));
        }
    }
}