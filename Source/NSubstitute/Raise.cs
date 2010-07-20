using System;
using NSubstitute.Core;

namespace NSubstitute
{
    public static class Raise
    {
        public static ActionWrapper<Action> Action()
        {
            return new ActionWrapper<Action>();
        }

        public static ActionWrapper<Action<T>> Action<T>(T argument)
        {
            return new ActionWrapper<Action<T>>(argument);
        }

        public static ActionWrapper<Action<T1, T2>> Action<T1, T2>(T1 argument1, T2 argument2)
        {
            return new ActionWrapper<Action<T1, T2>>(argument1, argument2);
        }

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
    }

    public class EventHandlerWrapper<TEventArgs> where TEventArgs : EventArgs
    {
        object _sender;
        readonly EventArgs _eventArgs;

        public EventHandlerWrapper(): this(null, null)
        {
        }

        public EventHandlerWrapper(EventArgs eventArgs): this(null, eventArgs)
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
            context.RaiseEventForNextCall(delegate (ICall call)
                                              {
                                                  if (sender == null)
                                                      sender = call.Target();
                                                  if (eventArgs == null && typeof(TEventArgs) == typeof(EventArgs)) 
                                                      eventArgs = EventArgs.Empty;

                                                  return new[]{sender, eventArgs};
                                              });
        }
    }

    public class ActionWrapper<T>
    {
        readonly object[] _arguments;

        public ActionWrapper(params object[] arguments)
        {
            _arguments = arguments;
        }

        public static implicit operator T(ActionWrapper<T> wrapper)
        {
            RaiseEvent(wrapper);
            return default(T);
        }

        static void RaiseEvent(ActionWrapper<T> wrapper)
        {
            var arguments = wrapper._arguments;
            var context = SubstitutionContext.Current;
            context.RaiseEventForNextCall(callRouter => arguments);
        }
    }
}