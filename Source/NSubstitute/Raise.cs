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

        /// <summary>
        /// Raise an event for an <see>EventHandler</see> event using empty <see>EventArgs</see>.
        /// </summary>
        /// <returns></returns>
        public static EventHandlerWrapper<EventArgs> Event()
        {
            return new EventHandlerWrapper<EventArgs>();
        }

        public static DelegateEventWrapper<THandler> Event<THandler>(params object[] arguments)
        {
            return new DelegateEventWrapper<THandler>(arguments);
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
            _arguments = WorkOutRequiredArguments(arguments);
        }

        object[] WorkOutRequiredArguments(object[] providedArgs)
        {
            providedArgs = providedArgs ?? new object[0];
            var requiredArgs = typeof(T).GetMethod("Invoke").GetParameters();

            if (providedArgs.Length == 0 && LooksLikeAnEventStyleCall(requiredArgs))
            {
                return new object[] { this, GetDefaultForEventArgType(requiredArgs[1].ParameterType)};
            }

            if (!RequiredArgsHaveBeenProvided(providedArgs, requiredArgs))
            {
                var message = string.Format(
                           "Cannot raise event with the provided arguments. Use Raise.Event<{0}>({1}) to raise this event.",
                           typeof(T).Name,
                           string.Join(", ", requiredArgs.Select(x => x.ParameterType.Name).ToArray())
                );
                throw new ArgumentException(message);
            }

            return providedArgs;
        }

        bool RequiredArgsHaveBeenProvided(object[] providedArgs, ParameterInfo[] requiredArgs)
        {
            if (providedArgs.Length != requiredArgs.Length) return false;
            for (var i = 0; i < providedArgs.Length; i++)
            {
                var requiredArgType = requiredArgs[i].ParameterType;
                var providedArgType = providedArgs[i].GetType();
                if (!requiredArgType.IsAssignableFrom(providedArgType))
                {
                    return false;
                }
            }
            return true;
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
