using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class CallFormatter : ICallFormatter
    {
        private readonly IEnumerable<IMethodInfoFormatter> _methodInfoFormatters;
        private readonly IArgumentFormatInfoFactory _argumentFormatInfoFactory;

        public CallFormatter(IArgumentsFormatter argumentsFormatter, IArgumentFormatInfoFactory argumentFormatInfoFactory)
        {
            _argumentFormatInfoFactory = argumentFormatInfoFactory;
            _methodInfoFormatters = new IMethodInfoFormatter[] { 
                new PropertyCallFormatter(argumentsFormatter), 
                new EventCallFormatter(argumentsFormatter, EventCallFormatter.IsSubscription), 
                new EventCallFormatter(argumentsFormatter, EventCallFormatter.IsUnsubscription), 
                new MethodFormatter(argumentsFormatter) };
        }

        public string Format(ICall call, ICallSpecification withRespectToCallSpec)
        {
            return Format(call.GetMethodInfo(), call.GetArguments(), withRespectToCallSpec.NonMatchingArguments(call));
        }

        public string Format(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<ArgumentMatchInfo> nonMatchingArguments)
        {
            IEnumerable<IArgumentFormatInfo> argumentFormatInfos = CreateArgumentFormatInfos(methodInfoOfCall, arguments, nonMatchingArguments);

            return _methodInfoFormatters
                        .First(x => x.CanFormat(methodInfoOfCall))
                        .Format(methodInfoOfCall, argumentFormatInfos);
        }

        private IEnumerable<IArgumentFormatInfo> CreateArgumentFormatInfos(MethodInfo methodInfoOfCall, IEnumerable<object> arguments, IEnumerable<ArgumentMatchInfo> nonMatchingArguments)
        {
            IEnumerable<int> argumentsToHighlight = nonMatchingArguments.Select(x => x.Index);

            return _argumentFormatInfoFactory.CreateArgumentFormatInfos(methodInfoOfCall, arguments, argumentsToHighlight);
        }
    }
}