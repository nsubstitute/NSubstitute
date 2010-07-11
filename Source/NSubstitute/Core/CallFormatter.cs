using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallFormatter : ICallFormatter
    {
        private readonly IEnumerable<IMethodInfoFormatter> _methodInfoFormatters;

        public CallFormatter(IArgumentsFormatter argumentsFormatter)
        {
            _methodInfoFormatters = new IMethodInfoFormatter[] { new PropertyCallFormatter(argumentsFormatter), new MethodFormatter(argumentsFormatter) };
        }

        public string Format(ICall call, ICallSpecification withRespectToCallSpec)
        {
            return Format(call.GetMethodInfo(), call.GetArguments(), withRespectToCallSpec.NonMatchingArgumentIndicies(call));
        }

        public string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            return _methodInfoFormatters
                        .First(x => x.CanFormat(methodInfoOfCall))
                        .Format(methodInfoOfCall, arguments, argumentsToHighlight);
        }
    }
}