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
            string genericInfo = null;
            var propertyInfoFromMethod = methodInfoOfCall.GetPropertyFromSetterCallOrNull();
            if (propertyInfoFromMethod != null)
            {
                return propertyInfoFromMethod.Name + " = " + FormatArgs(arguments, argumentsToHighlight);
            }
            if (methodInfoOfCall.IsGenericMethod)
            {
                var genericArgs = methodInfoOfCall.GetGenericArguments();
                genericInfo = "<" + string.Join(", ", genericArgs.Select(x => x.Name).ToArray()) + ">";
            }
            return string.Format("{0}{1}({2})", methodInfoOfCall.Name, genericInfo, FormatArgs(arguments, argumentsToHighlight));
        }

        private string FormatArgs(IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            return string.Join(", ", arguments.Select( (argument, index) => FormatArg(argument, argumentsToHighlight.Contains(index))).ToArray());
        }

        private string FormatArg(object argument, bool highlight)
        {
            var argAsString = _argumentFormatter.Format(argument);
            return highlight ? "*" + argAsString + "*" : argAsString;
        }
    }
}