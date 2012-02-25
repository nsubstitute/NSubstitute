using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

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
            var argumentsWithParamsSplit = SplitParamsArgumentIntoValues(methodInfoOfCall, arguments);
            var argumentsToHighlightWithParamsSplit = SplitParamsHighlights(methodInfoOfCall, arguments, argumentsToHighlight);
            string genericInfo = CreateGenericInfo(methodInfoOfCall);

            return string.Format("{0}{1}({2})", methodInfoOfCall.Name, genericInfo, _argumentsFormatter.Format(argumentsWithParamsSplit, argumentsToHighlightWithParamsSplit));
        }

        private IEnumerable<object> SplitParamsArgumentIntoValues(MethodInfo methodInfoOfCall, IEnumerable<object> arguments)
        {
            if(HasParamsArgument(methodInfoOfCall))
            {
                return CreateArgumentsWithParamsValues(arguments.ToList());
            }

            return arguments;
        }

        private IEnumerable<int> SplitParamsHighlights(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            if(argumentsToHighlight.Any() && HasParamsArgument(methodInfoOfCall))
            {
                return CreateHighlightsWithParamsValues(arguments.ToList(), argumentsToHighlight.ToList());
            }

            return argumentsToHighlight;
        }

        private IEnumerable<int> CreateHighlightsWithParamsValues(List<object> arguments, List<int> argumentsToHighlight)
        {
            int indexOfParamsArgument = GetIndexOfParamsArgument(arguments);
            int numberOfParamsValues = GetNumberOfParamsValues(arguments);

            if (argumentsToHighlight.Contains(indexOfParamsArgument))
            {
                ReplaceParamsArgumentIndexWithParamsValuesIndeces(argumentsToHighlight, indexOfParamsArgument, numberOfParamsValues);
            }

            return argumentsToHighlight;
        }

        private void ReplaceParamsArgumentIndexWithParamsValuesIndeces(List<int> argumentsToHighlight, int indexOfParamsArgument, int numberOfParamsValues)
        {
            argumentsToHighlight.Remove(indexOfParamsArgument);

            for (int i = 0; i < numberOfParamsValues; i++)
            {
                argumentsToHighlight.Add(indexOfParamsArgument + i);
            }
        }

        private int GetNumberOfParamsValues(List<object> arguments)
        {
            return GetParamsValues(arguments).Count();
        }

        private IEnumerable<object> CreateArgumentsWithParamsValues(List<object> arguments)
        {
            int indexOfParamsArgument = GetIndexOfParamsArgument(arguments);
            IEnumerable<object> paramsValues = GetParamsValues(arguments);

            arguments.RemoveAt(indexOfParamsArgument);
            arguments.InsertRange(indexOfParamsArgument, paramsValues);

            return arguments;
        }

        private int GetIndexOfParamsArgument(IEnumerable<object> arguments)
        {
            //Params is always the last argument of the method call.
            return arguments.Count() - 1;
        }

        private IEnumerable<object> GetParamsValues(List<object> arguments)
        {
            return (object[]) arguments[GetIndexOfParamsArgument(arguments)];
        }

        private bool HasParamsArgument(MethodInfo methodInfoOfCall)
        {
            return methodInfoOfCall.GetParameters().Any(p => p.IsParams());
        }

        private string CreateGenericInfo(MethodInfo methodInfoOfCall)
        {
            string genericInfo = null;

            if (methodInfoOfCall.IsGenericMethod)
            {
                var genericArgs = methodInfoOfCall.GetGenericArguments();
                genericInfo = "<" + string.Join(", ", genericArgs.Select(x => x.Name).ToArray()) + ">";
            }

            return genericInfo;
        }
    }
}