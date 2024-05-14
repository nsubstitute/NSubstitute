namespace NSubstitute.Core.Events;

public class EventHandlerWrapper<TEventArgs>(object? sender, EventArgs? eventArgs) : RaiseEventWrapper where TEventArgs : EventArgs
{
    private readonly object? _sender = sender;
    private readonly EventArgs? _eventArgs = eventArgs;
    protected override string RaiseMethodName => "Raise.EventWith";

    public EventHandlerWrapper() : this(null, null) { }

    public EventHandlerWrapper(EventArgs? eventArgs) : this(null, eventArgs) { }

    // Disable nullability for client API, so it does not affect clients.
#nullable disable annotations
    public static implicit operator EventHandler(EventHandlerWrapper<TEventArgs> wrapper)
    {
        RaiseEvent(wrapper);
        return null;
    }

    public static implicit operator EventHandler<TEventArgs>(EventHandlerWrapper<TEventArgs> wrapper)
    {
        RaiseEvent(wrapper);
        return null;
    }
#nullable restore annotations

    protected override object[] WorkOutRequiredArguments(ICall call)
    {
        var sender = _sender ?? call.Target();
        var eventArgs = _eventArgs ?? GetDefaultForEventArgType(typeof(TEventArgs));
        return [sender, eventArgs];
    }
}