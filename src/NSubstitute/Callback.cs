using System;
using System.Collections.Concurrent;
using NSubstitute.Callbacks;
using NSubstitute.Core;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute
{
    /// <summary>
    /// Perform this chain of callbacks and/or always callback when called.
    /// </summary>
    public class Callback
    {
        /// <summary>
        /// Perform as first in chain of callback when called.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
        public static ConfiguredCallback First(Action<CallInfo> doThis)
        {
            return new ConfiguredCallback().Then(doThis);
        }

        /// <summary>
        /// Perform this action always when callback is called.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
        public static Callback Always(Action<CallInfo> doThis)
        {
            return new ConfiguredCallback().AndAlways(doThis);
        }

        /// <summary>
        /// Throw exception returned by function as first callback in chain of callback when called.
        /// </summary>
        /// <param name="throwThis"></param>
        /// <returns></returns>
        public static ConfiguredCallback FirstThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return new ConfiguredCallback().ThenThrow(throwThis);
        }

        /// <summary>
        /// Throw this exception as first callback in chain of callback when called.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ConfiguredCallback FirstThrow<TException>(TException exception) where TException : Exception
        {
            return new ConfiguredCallback().ThenThrow(info => exception);
        }

        /// <summary>
        /// Throw exception returned by function always when callback is called.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="throwThis">The throw this.</param>
        /// <returns></returns>
        public static Callback AlwaysThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return new ConfiguredCallback().AndAlways(ToCallback(throwThis));
        }

        /// <summary>
        /// Throw this exception always when callback is called.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static Callback AlwaysThrow<TException>(TException exception) where TException : Exception
        {
            return AlwaysThrow(_ => exception);
        }

        protected static Action<CallInfo> ToCallback<TException>(Func<CallInfo, TException> throwThis)
            where TException : notnull, Exception
        {
            return ci => { if (throwThis != null) throw throwThis(ci); };
        }

        internal Callback() { }
        private readonly ConcurrentQueue<Action<CallInfo>> callbackQueue = new ConcurrentQueue<Action<CallInfo>>();
        private Action<CallInfo> alwaysDo = x => { };
        private Action<CallInfo> keepDoing = x => { };

        protected void AddCallback(Action<CallInfo> doThis)
        {
            callbackQueue.Enqueue(doThis);
        }

        protected void SetAlwaysDo(Action<CallInfo> always)
        {
            alwaysDo = always ?? (_ => { });
        }

        protected void SetKeepDoing(Action<CallInfo> keep)
        {
            keepDoing = keep ?? (_ => { });
        }

        public void Call(CallInfo callInfo)
        {
            try
            {
                CallFromStack(callInfo);
            }
            finally
            {
                alwaysDo(callInfo);
            }
        }

        private void CallFromStack(CallInfo callInfo)
        {
            if (callbackQueue.TryDequeue(out var callback))
            {
                callback(callInfo);
            }
            else
            {
                keepDoing(callInfo);
            }
        }
    }
}