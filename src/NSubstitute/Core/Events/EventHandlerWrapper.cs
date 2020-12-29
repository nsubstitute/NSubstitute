using System;

namespace NSubstitute.Core.Events
{
    public class EventHandlerWrapper<TEventArgs> : RaiseEventWrapper where TEventArgs : EventArgs
    {
        private readonly object? _sender;
        private readonly EventArgs? _eventArgs;
        protected override string RaiseMethodName => "Raise.EventWith";

        public EventHandlerWrapper() : this(null, null) { }

        public EventHandlerWrapper(EventArgs? eventArgs) : this(null, eventArgs) { }

        public EventHandlerWrapper(object? sender, EventArgs? eventArgs)
        {
            _sender = sender;
            _eventArgs = eventArgs;
        }

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
            return new[] { sender, eventArgs };
        }
    }
}