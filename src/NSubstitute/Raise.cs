using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Events;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

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
            var normalizedArgs = FixParamsArrayAmbiguity(arguments, typeof(THandler));
            return new DelegateEventWrapper<THandler>(normalizedArgs);
        }

        /// <summary>
        /// If delegate takes single parameter of array type, it's impossible to distinguish
        /// whether input array represents all arguments, or the first argument only.
        /// If we find that ambiguity might happen, we wrap user input in an extra array.
        /// </summary>
        private static object[] FixParamsArrayAmbiguity(object[] arguments, Type delegateType)
        {
            ParameterInfo[] invokeMethodParameters = delegateType.GetInvokeMethod().GetParameters();
            if (invokeMethodParameters.Length != 1)
            {
                return arguments;
            }

            Type singleParameterType = invokeMethodParameters[0].ParameterType;
            if (!singleParameterType.IsArray)
            {
                return arguments;
            }

            // Check if native non-params syntax was used.
            // This way actual value is already wrapped in array and we don't need to extra-wrap it.
            if (arguments.Length == 1 && singleParameterType.IsInstanceOfType(arguments[0]))
            {
                return arguments;
            }

            if (singleParameterType.IsInstanceOfType(arguments))
            {
                return new object[] {arguments};
            }

            return arguments;
        }
    }
}
