using System.Reflection;

namespace NSubstitute.Core;

public class EventCallFormatter(Func<MethodInfo, Predicate<EventInfo>> eventsToFormat) : IMethodInfoFormatter
{
    public static readonly Func<MethodInfo, Predicate<EventInfo>> IsSubscription =
        call => (eventInfo => eventInfo.GetAddMethod() == call);
    public static readonly Func<MethodInfo, Predicate<EventInfo>> IsUnsubscription =
        call => (eventInfo => eventInfo.GetRemoveMethod() == call);
    private readonly string _eventOperator = eventsToFormat == IsSubscription ? "+=" : "-=";

    public bool CanFormat(MethodInfo methodInfo)
    {
        return methodInfo.DeclaringType!.GetEvents().Any(x => eventsToFormat(methodInfo)(x));
    }

    public string Format(MethodInfo methodInfo, IEnumerable<string> arguments)
    {
        var eventInfo = methodInfo.DeclaringType!.GetEvents().First(x => eventsToFormat(methodInfo)(x));
        return Format(eventInfo, _eventOperator, arguments);
    }

    private string Format(EventInfo eventInfo, string eventOperator, IEnumerable<string> arguments) =>
        $"{eventInfo.Name} {eventOperator} {arguments.Join(", ")}";
}