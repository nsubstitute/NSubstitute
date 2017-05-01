using System;

namespace NSubstitute.Core.Events
{
    public class EventHandlerWrapper<TEventArgs> : RaiseEventWrapper where TEventArgs : EventArgs
    {
        readonly object _sender;
        readonly EventArgs _eventArgs;
        protected override string RaiseMethodName { get { return "Raise.EventWith"; } }

        public EventHandlerWrapper() : this(null, null) { }

        public EventHandlerWrapper(EventArgs eventArgs) : this(null, eventArgs) { }

        public EventHandlerWrapper(object sender, EventArgs eventArgs)
        {
            _sender = sender;
            _eventArgs = eventArgs;
        }

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

        protected override object[] WorkOutRequiredArguments(ICall call)
        {
            var sender = _sender;
            var eventArgs = _eventArgs;
            if (sender == null)
                sender = call.Target();
            if (eventArgs == null)
                eventArgs = GetDefaultForEventArgType(typeof(TEventArgs));
            return new[] { sender, eventArgs };
        }
    }
}