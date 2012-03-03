using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

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

        public string Format(MethodInfo methodInfo, IEnumerable<IArgumentFormatInfo> argumentFormatInfos)
        {
            var propertyInfo = GetPropertyFromGetterOrSetterCall(methodInfo);
            var numberOfIndexParams = propertyInfo.GetIndexParameters().Length;
            var propertyName =
                (numberOfIndexParams == 0)
                    ? propertyInfo.Name
                    : FormatPropertyIndexer(numberOfIndexParams, argumentFormatInfos);

            return propertyName + FormatArgsAfterIndexParamsAsSetterArgs(numberOfIndexParams, argumentFormatInfos);
        }

        private PropertyInfo GetPropertyFromGetterOrSetterCall(MethodInfo methodInfoOfCall)
        {
            return methodInfoOfCall.GetPropertyFromSetterCallOrNull() ?? methodInfoOfCall.GetPropertyFromGetterCallOrNull();
        }

        private string FormatPropertyIndexer(int numberofIndexParameters, IEnumerable<IArgumentFormatInfo> argumentFormatInfos)
        {
            return "this[" + _argumentsFormatter.Format(argumentFormatInfos.Take(numberofIndexParameters)) + "]";
        }

        private bool OnlyHasIndexParameterArgs(int numberOfIndexParameters, IEnumerable<IArgumentFormatInfo> argumentFormatInfos)
        {
            return numberOfIndexParameters >= argumentFormatInfos.Count();
        }

        private string FormatArgsAfterIndexParamsAsSetterArgs(int numberOfIndexParameters, IEnumerable<IArgumentFormatInfo> argumentFormatInfos)
        {
            if (OnlyHasIndexParameterArgs(numberOfIndexParameters, argumentFormatInfos)) return string.Empty;
            return " = " + _argumentsFormatter.Format(argumentFormatInfos.Skip(numberOfIndexParameters));
        }
    }
}