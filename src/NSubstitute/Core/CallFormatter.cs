using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class CallFormatter : IMethodInfoFormatter
    {
        internal static IMethodInfoFormatter Default { get; } = new CallFormatter();

        private readonly IEnumerable<IMethodInfoFormatter> _methodInfoFormatters;

        public CallFormatter()
        {
            _methodInfoFormatters = new IMethodInfoFormatter[] {
                new PropertyCallFormatter(),
                new EventCallFormatter(EventCallFormatter.IsSubscription),
                new EventCallFormatter(EventCallFormatter.IsUnsubscription),
                new MethodFormatter() };
        }

        public bool CanFormat(MethodInfo methodInfo) => true;

        public string Format(MethodInfo methodInfoOfCall, IEnumerable<string> formattedArguments)
        {
            return _methodInfoFormatters
                        .First(x => x.CanFormat(methodInfoOfCall))
                        .Format(methodInfoOfCall, formattedArguments);
        }
    }
}