using System;

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
            return new ActionWrapper<Action<T>>();
        }

        public static ActionWrapper<Action<T1, T2>> Action<T1, T2>(T1 argument1, T2 argument2)
        {
            return new ActionWrapper<Action<T1, T2>>();
        }

        public static EventHandlerWrapper<EventHandler<TEventArgs>> Event<TEventArgs>(object sender, TEventArgs arguments) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<EventHandler<TEventArgs>>(sender, arguments);
        }

        public static EventHandlerWrapper<EventHandler<TEventArgs>> Event<TEventArgs>(TEventArgs arguments) where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<EventHandler<TEventArgs>>();
        }

        public static EventHandlerWrapper<EventHandler> Event(object sender, EventArgs arguments)
        {
            return new EventHandlerWrapper<EventHandler>(sender, arguments);
        }

        public static EventHandlerWrapper<EventHandler> Event(EventArgs arguments)
        {
            return new EventHandlerWrapper<EventHandler>();
        }

        public static EventHandlerWrapper<EventHandler<TEventArgs>> Event<TEventArgs>() where TEventArgs : EventArgs
        {
            return new EventHandlerWrapper<EventHandler<TEventArgs>>();
        }

/*
        public static EventHandlerWrapper<EventHandler> Event()
        {
            return new EventHandlerWrapper<EventHandler>();
        }

  */
        public static EventHandlerWrapper<object> Event()
        {
            return new EventHandlerWrapper<object>();
        }
    }

    public class EventHandlerWrapper<T>
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

        public static implicit operator T(EventHandlerWrapper<T> wrapper)
        {
            RaiseEvent(wrapper);
            return default(T);
        }

        public static implicit operator EventHandler(EventHandlerWrapper<T> wrapper)
        {
            RaiseEvent(wrapper);
            return null;
        }

        public static implicit operator EventHandler<EventArgs>(EventHandlerWrapper<T> wrapper)
        {
            RaiseEvent(wrapper);
            return null;
        }

        static void RaiseEvent(EventHandlerWrapper<T> wrapper)
        {
            var sender = wrapper._sender;
            var eventArgs = wrapper._eventArgs;
            var context = SubstitutionContext.Current;
            context.RaiseEventForNextCall(delegate (ICall call)
                                              {
                                                  if (sender == null)
                                                      sender = call;
                                                  if (eventArgs == null)
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