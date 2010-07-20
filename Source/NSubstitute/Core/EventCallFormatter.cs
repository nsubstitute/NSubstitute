using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public class EventCallFormatter : IMethodInfoFormatter
    {
        public static readonly Func<MethodInfo, Predicate<EventInfo>> IsSubscription = call => (eventInfo => eventInfo.GetAddMethod() == call);
        public static readonly Func<MethodInfo, Predicate<EventInfo>> IsUnsubscription = call => (eventInfo => eventInfo.GetRemoveMethod() == call);

        private readonly IArgumentsFormatter _argumentsFormatter;
        private readonly Func<MethodInfo, Predicate<EventInfo>> _eventsToFormat;
        private readonly string _eventOperator;

        public EventCallFormatter(IArgumentsFormatter argumentsFormatter, Func<MethodInfo, Predicate<EventInfo>> eventsToFormat, string eventOperator)
        {
            _argumentsFormatter = argumentsFormatter;
            _eventsToFormat = eventsToFormat;
            _eventOperator = eventOperator;
        }

        public bool CanFormat(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType.GetEvents().Any(x => _eventsToFormat(methodInfo)(x));
        }

        public string Format(MethodInfo methodInfo, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            var eventInfo = methodInfo.DeclaringType.GetEvents().First(x => _eventsToFormat(methodInfo)(x));
            return Format(eventInfo, _eventOperator, arguments, argumentsToHighlight);
        }

        private string Format(EventInfo eventInfo, string eventOperator, IEnumerable<object> arguments, IEnumerable<int> argumentsToHighlight)
        {
            return string.Format("{0} {1} {2}", eventInfo.Name, eventOperator, _argumentsFormatter.Format(arguments, argumentsToHighlight));
        }
    }
}