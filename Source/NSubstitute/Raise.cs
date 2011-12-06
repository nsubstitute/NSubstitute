using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;

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
        /// NSubstitute will try and provide reasonble defaults.
        /// </summary>
        public static DelegateEventWrapper<THandler> Event<THandler>(params object[] arguments)
        {
            return new DelegateEventWrapper<THandler>(arguments);
        }
    }

    public abstract class RaiseEventWrapper
    {
        protected abstract object[] WorkOutRequiredArguments(ICall call);
        protected abstract string RaiseMethodName { get; }

        protected EventArgs GetDefaultForEventArgType(Type type)
        {
            if (type == typeof(EventArgs)) return EventArgs.Empty;
            var defaultConstructor = GetDefaultConstructor(type);
            if (defaultConstructor == null)
            {
                var message = string.Format(
                    "Cannot create {0} for this event as it has no default constructor. " +
                    "Provide arguments for this event by calling {1}({0})."
                    , type.Name, RaiseMethodName);
                throw new CannotCreateEventArgsException(message);
            }
            return (EventArgs)defaultConstructor.Invoke(new object[0]);
        }

        static ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        protected static void RaiseEvent(RaiseEventWrapper wrapper)
        {
            var context = SubstitutionContext.Current;
            context.RaiseEventForNextCall(call => wrapper.WorkOutRequiredArguments(call));
        }
    }

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

    public class DelegateEventWrapper<T> : RaiseEventWrapper
    {
        readonly object[] _providedArguments;
        protected override string RaiseMethodName { get { return "Raise.Event"; } }

        public DelegateEventWrapper(params object[] arguments)
        {
            _providedArguments = arguments ?? new object[0];
        }

        public static implicit operator T(DelegateEventWrapper<T> wrapper)
        {
            RaiseEvent(wrapper);
            return default(T);
        }

        protected override object[] WorkOutRequiredArguments(ICall call)
        {
            var requiredArgs = typeof(T).GetMethod("Invoke").GetParameters();

            if (_providedArguments.Length < 2 && LooksLikeAnEventStyleCall(requiredArgs))
            {
                return WorkOutSenderAndEventArgs(requiredArgs[1].ParameterType, call);
            }

            if (!RequiredArgsHaveBeenProvided(_providedArguments, requiredArgs))
            {
                ThrowBecauseRequiredArgsNotProvided(requiredArgs);
            }

            return _providedArguments;
        }

        bool LooksLikeAnEventStyleCall(ParameterInfo[] parameters)
        {
            return parameters.Length == 2 &&
                   parameters[0].ParameterType == typeof(object) &&
                   typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType);
        }

        object[] WorkOutSenderAndEventArgs(Type eventArgsType, ICall call)
        {
            object sender;
            object eventArgs;
            if (_providedArguments.Length == 0)
            {
                sender = call.Target();
                eventArgs = GetDefaultForEventArgType(eventArgsType);
            }
            else if (_providedArguments[0].IsCompatibleWith(eventArgsType))
            {
                sender = call.Target();
                eventArgs = _providedArguments[0];
            }
            else
            {
                sender = _providedArguments[0];
                eventArgs = GetDefaultForEventArgType(eventArgsType);
            }
            return new[] { sender, eventArgs };
        }

        bool RequiredArgsHaveBeenProvided(object[] providedArgs, ParameterInfo[] requiredArgs)
        {
            if (providedArgs.Length != requiredArgs.Length) return false;
            for (var i = 0; i < providedArgs.Length; i++)
            {
                var requiredArgType = requiredArgs[i].ParameterType;
                var providedArg = providedArgs[i];
                if (!providedArg.IsCompatibleWith(requiredArgType)) return false;
            }
            return true;
        }

        void ThrowBecauseRequiredArgsNotProvided(ParameterInfo[] requiredArgs)
        {
            var message = string.Format(
                "Cannot raise event with the provided arguments. Use Raise.Event<{0}>({1}) to raise this event.",
                typeof(T).Name,
                string.Join(", ", requiredArgs.Select(x => x.ParameterType.Name).ToArray())
                );
            throw new ArgumentException(message);
        }
    }
}
