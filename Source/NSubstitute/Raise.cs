using System;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute
{
    public static class Raise
    {
        public static EventHandlerWrapper<TEventArgs> Event<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>(sender, eventArgs);
        }

        public static EventHandlerWrapper<TEventArgs> Event<TEventArgs>(TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>(eventArgs);
        }

        public static EventHandlerWrapper<TEventArgs> Event<TEventArgs>() where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<TEventArgs>(null);
        }

        public static EventHandlerWrapper<EventArgs> Event()
        {
            return new EventHandlerWrapper<EventArgs>();
        }

        public static DelegateEventWrapper<T> Event<T>(params object[] arguments)
        {
            return new DelegateEventWrapper<T>(arguments);
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
                    "Provide arguments for this event by calling Raise.Event(instanceOf{0})."
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
            _arguments = arguments;
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
