using System;
using NSubstitute.Core.Events;

namespace NSubstitute
{
    public static class Raise
    {
        /// <summary>
        /// Raise an event for an <c>EventHandler&lt;TEventArgs&gt;</c> event with the provided <paramref name="sender"/> and <paramref name="eventArgs"/>.
        /// </summary>
        public static EventHandlerWrapper<TEventArgs> EventWith<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>(sender, eventArgs);
        }

        /// <summary>
        /// Raise an event for an <c>EventHandler&lt;TEventArgs&gt;</c> event with the substitute as the sender and the provided <paramref name="eventArgs" />.
        /// </summary>
        public static EventHandlerWrapper<TEventArgs> EventWith<TEventArgs>(TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>(eventArgs);
        }

        /// <summary>
        /// Raise an event for an <c>EventHandler&lt;EventArgsT&gt;</c> event with the substitute as the sender
        /// and with a default instance of <typeparamref name="TEventArgs" />.
        /// </summary>
        public static EventHandlerWrapper<TEventArgs> EventWith<TEventArgs>() where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>();
        }

        /// <summary>
        /// Raise an event for an <c>EventHandler</c> or <c>EventHandler&lt;EventArgs&gt;</c> event with the substitute
        /// as the sender and with empty <c>EventArgs</c>.
        /// </summary>
        public static EventHandlerWrapper<EventArgs> Event()
        {
            return new EventHandlerWrapper<EventArgs>();
        }

        /// <summary>
        /// Raise an event of type <typeparamref name="THandler" /> with the provided arguments. If no arguments are provided
        /// NSubstitute will try to provide reasonable defaults.
        /// </summary>
        public static DelegateEventWrapper<THandler> Event<THandler>(params object[] arguments)
        {
            return new DelegateEventWrapper<THandler>(arguments);
        }
    }
}
