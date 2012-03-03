using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class MethodFormatter : IMethodInfoFormatter
    {
        IArgumentsFormatter _argumentsFormatter;
        IArgumentParamsExtractorFactory _paramsExtractorFactory;

        public MethodFormatter(IArgumentsFormatter argumentsFormatter)
        {
            _argumentsFormatter = argumentsFormatter;
            _paramsExtractorFactory = new ArgumentParamsExtractorFactory();
        }

        public bool CanFormat(MethodInfo methodInfo)
        {
            return true;
        }

        public string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            IArgumentParamsExtractor paramsExtractor = _paramsExtractorFactory.Create(methodInfoOfCall, arguments, argumentsToHighlight);

            var argumentsWithParamsExtracted = paramsExtractor.GetWithExtractedArguments(arguments);
            var argumentsToHighlightWithParamsExtracted = paramsExtractor.GetWithExtractedArgumentsToHighlight(arguments, argumentsToHighlight);
            string genericInfo = CreateGenericInfo(methodInfoOfCall);

            return string.Format("{0}{1}({2})", methodInfoOfCall.Name, genericInfo, _argumentsFormatter.Format(argumentsWithParamsExtracted, argumentsToHighlightWithParamsExtracted));
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