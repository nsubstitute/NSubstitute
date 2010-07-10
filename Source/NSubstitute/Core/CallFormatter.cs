using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallFormatter : ICallFormatter
    {
        private readonly IArgumentFormatter _argumentFormatter;

        public CallFormatter(IArgumentFormatter argumentFormatter)
        {
            _argumentFormatter = argumentFormatter;
        }

        public string Format(ICall call, ICallSpecification withRespectToCallSpec)
        {
            return Format(call.GetMethodInfo(), call.GetArguments(), withRespectToCallSpec.NonMatchingArgumentIndicies(call));
        }

        public string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            var propertyInfoFromCall = GetPropertyFromGetterOrSetterCall(methodInfoOfCall);
            if (propertyInfoFromCall != null)
            {
                return FormatPropertyCall(propertyInfoFromCall, argumentsToHighlight, arguments);
            }
            return FormatMethod(methodInfoOfCall, argumentsToHighlight, arguments);
        }

        private PropertyInfo GetPropertyFromGetterOrSetterCall(MethodInfo methodInfoOfCall)
        {
            return methodInfoOfCall.GetPropertyFromSetterCallOrNull() ?? methodInfoOfCall.GetPropertyFromGetterCallOrNull();
        }

        private string FormatMethod(MethodInfo methodInfoOfCall, IEnumerable<int> argumentsToHighlight, IEnumerable<object> arguments)
        {
            string genericInfo = null;
            if (methodInfoOfCall.IsGenericMethod)
            {
                var genericArgs = methodInfoOfCall.GetGenericArguments();
                genericInfo = "<" + string.Join(", ", genericArgs.Select(x => x.Name).ToArray()) + ">";
            }
            return string.Format("{0}{1}({2})", methodInfoOfCall.Name, genericInfo, FormatArgs(arguments, argumentsToHighlight));
        }

        private string FormatPropertyCall(PropertyInfo propertyInfo, IEnumerable<int> argumentsToHighlight, IEnumerable<object> arguments)
        {
            var numberOfIndexParams = propertyInfo.GetIndexParameters().Length;
            var propertyName =
                (numberOfIndexParams == 0)
                ? propertyInfo.Name
                : FormatPropertyIndexer(numberOfIndexParams, argumentsToHighlight, arguments);

            return propertyName + FormatArgsAfterIndexParamsAsSetterArgs(numberOfIndexParams, argumentsToHighlight, arguments);
        }

        private string FormatPropertyIndexer(int numberofIndexParameters, IEnumerable<int> argumentsToHighlight, IEnumerable<object> arguments)
        {
            return "this[" + FormatArgs(arguments.Take(numberofIndexParameters), argumentsToHighlight) + "]";
        }

        private bool OnlyHasIndexParameterArgs(int numberOfIndexParameters, IEnumerable<object> arguments)
        {
            return numberOfIndexParameters >= arguments.Count();
        }

        private string FormatArgsAfterIndexParamsAsSetterArgs(int numberOfIndexParameters, IEnumerable<int> argumentsToHighlight, IEnumerable<object> arguments)
        {
            if (OnlyHasIndexParameterArgs(numberOfIndexParameters, arguments)) return string.Empty;
            return " = " + FormatArgs(arguments.Skip(numberOfIndexParameters), argumentsToHighlight.Select(x => x - numberOfIndexParameters));
        }

        private string FormatArgs(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            return string.Join(", ", arguments.Select((argument, index) => FormatArg(argument, argumentsToHighlight.Contains(index))).ToArray());
        }

        private string FormatArg(object argument, bool highlight)
        {
            var argAsString = _argumentFormatter.Format(argument);
            return highlight ? "*" + argAsString + "*" : argAsString;
        }
    }
}