using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute
{
    public static class Raise
    {
        public static EventHandlerWrapper<TEventArgs> EventWith<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>(sender, eventArgs);
        }

        public static EventHandlerWrapper<TEventArgs> EventWith<TEventArgs>(TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>(eventArgs);
        }

        public static EventHandlerWrapper<TEventArgs> EventWith<TEventArgs>() where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>();
        }

        public static EventHandlerWrapper<EventArgs> EventWithEmptyEventArgs()
        {
            return new EventHandlerWrapper<EventArgs>();
        }

        public static EventHandlerWrapper<EventArgs> Event()
        {
            return new EventHandlerWrapper<EventArgs>();
        }

        public static DelegateEventWrapper<THandler> Event<THandler>(params object[] arguments)
        {
            return new DelegateEventWrapper<THandler>(arguments);
        }

        public static DelegateEventWrapper<Action> Action()
        {
            return new DelegateEventWrapper<Action>();
        }

        public static DelegateEventWrapper<Action<T>> Action<T>(T argument)
        {
            return new DelegateEventWrapper<Action<T>>(argument);
        }

        public static DelegateEventWrapper<Action<T1, T2>> Action<T1, T2>(T1 argument1, T2 argument2)
        {
            return new DelegateEventWrapper<Action<T1, T2>>(argument1, argument2);
        }
    }

    public class EventHandlerWrapper<TEventArgs> where TEventArgs : EventArgs
    {
        object _sender;
        readonly EventArgs _eventArgs;

        public EventHandlerWrapper()
            : this(null, null)
        {
        }

        public EventHandlerWrapper(EventArgs eventArgs)
            : this(null, eventArgs)
        {
        }

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

        static void RaiseEvent(EventHandlerWrapper<TEventArgs> wrapper)
        {
            var sender = wrapper._sender;
            var eventArgs = wrapper._eventArgs;
            var context = SubstitutionContext.Current;
            context.RaiseEventForNextCall(delegate(ICall call)
                                              {
                                                  if (sender == null)
                                                      sender = call.Target();
                                                  if (eventArgs == null)
                                                      eventArgs = GetDefaultForEventArgType(typeof(TEventArgs));


                                                  return new[] { sender, eventArgs };
                                              });
        }

        private static EventArgs GetDefaultForEventArgType(Type type)
        {
            if (type == typeof(EventArgs)) return EventArgs.Empty;
            var defaultConstructor = GetDefaultConstructor(type);
            if (defaultConstructor == null)
            {
                var message = string.Format(
                    "Cannot create {0} for this event as it has no default constructor. " +
                    "Provide arguments for this event by calling Raise.EventWith(instanceOf{0})."
                    , type.Name);
                throw new CannotCreateEventArgsException(message);
            }
            return (EventArgs)defaultConstructor.Invoke(new object[0]);
        }

        private static ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }
    }

    public class DelegateEventWrapper<T>
    {
        readonly object[] _arguments;

        public DelegateEventWrapper(params object[] arguments)
        {
            _arguments = DefaultArguments(arguments);
        }

        object[] DefaultArguments(object[] arguments)
        {
            arguments = arguments ?? new object[0];
            var parameters = typeof(T).GetMethod("Invoke").GetParameters();

            if (LooksLikeAnEventStyleCall(parameters) && arguments.Length == 0)
            {
                return new object[] { this, GetDefaultForEventArgType(parameters[1].ParameterType)};
            }

            if (arguments.Length != parameters.Length)
            {
                var message = string.Format(
@"Raising event of type {0} requires additional arguments.

Use Raise.Event<{0}>({1}) to raise this event.",
                           typeof(T).Name,
                           string.Join(", ", parameters.Select(x => x.ParameterType.Name).ToArray())
                );
                throw new ArgumentException(message);
            }

            return arguments;
        }

        bool LooksLikeAnEventStyleCall(ParameterInfo[] parameters)
        {
            return parameters.Length == 2 &&
                parameters[0].ParameterType == typeof(object) &&
                typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType);
        }

        private static EventArgs GetDefaultForEventArgType(Type type)
        {
            if (type == typeof(EventArgs)) return EventArgs.Empty;
            var defaultConstructor = GetDefaultConstructor(type);
            if (defaultConstructor == null)
            {
                var message = string.Format(
                    "Cannot create {0} for this event as it has no default constructor. " +
                    "Provide arguments for this event by calling Raise.EventWith(instanceOf{0})."
                    , type.Name);
                throw new CannotCreateEventArgsException(message);
            }
            return (EventArgs)defaultConstructor.Invoke(new object[0]);
        }

        private static ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        public static implicit operator T(DelegateEventWrapper<T> wrapper)
        {
            RaiseEvent(wrapper);
            return default(T);
        }

        static void RaiseEvent(DelegateEventWrapper<T> wrapper)
        {
            var arguments = wrapper._arguments;
            var context = SubstitutionContext.Current;
            context.RaiseEventForNextCall(callRouter => arguments);
        }
    }
}
