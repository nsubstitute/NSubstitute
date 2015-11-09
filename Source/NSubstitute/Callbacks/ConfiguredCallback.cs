using System;
using NSubstitute.Core;

namespace NSubstitute.Callbacks
{
    public class ConfiguredCallback : EndCallbackChain
    {
        internal ConfiguredCallback() { }

        /// <summary>
        /// Perform this action once in chain of called callbacks.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
        public ConfiguredCallback Then(Action<CallInfo> doThis)
        {
            AddCallback(doThis);
            return this;
        }

        /// <summary>
        /// Keep doing this action after the other callbacks have run.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
        public EndCallbackChain ThenKeepDoing(Action<CallInfo> doThis)
        {
            SetKeepDoing(doThis);
            return this;
        }

        /// <summary>
        /// Keep throwing this exception after the other callbacks have run.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="throwThis"></param>
        /// <returns></returns>
        public EndCallbackChain ThenKeepThrowing<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return ThenKeepDoing(ToCallback(throwThis));
        }

        /// <summary>
        /// Keep throwing this exception after the other callbacks have run.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="throwThis"></param>
        /// <returns></returns>
        public EndCallbackChain ThenKeepThrowing<TException>(TException throwThis) where TException : Exception
        {
            return ThenKeepThrowing(info => throwThis);
        }

        /// <summary>
        /// Throw exception returned by function once when called in a chain of callbacks.
        /// </summary>
        /// <typeparam name="TException">The type of the exception</typeparam>
        /// <param name="throwThis">Produce the exception to throw for a CallInfo</param>
        /// <returns></returns>
        public ConfiguredCallback ThenThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            AddCallback(ToCallback(throwThis));
            return this;
        }

        /// <summary>
        /// Throw this exception once when called in a chain of callbacks.
        /// </summary>
        /// <typeparam name="TException">The type of the exception</typeparam>
        /// <param name="exception">The exception to throw</param>
        /// <returns></returns>
        public ConfiguredCallback ThenThrow<TException>(TException exception) where TException : Exception
        {
            return ThenThrow(ci => exception);
        }
    }

    public class EndCallbackChain : Callback
    {
        internal EndCallbackChain() { }

        /// <summary>
        /// Perform the given action for every call.
        /// </summary>
        /// <param name="doThis">The action to perform for every call</param>
        /// <returns></returns>
        public Callback AndAlways(Action<CallInfo> doThis)
        {
            SetAlwaysDo(doThis);
            return this;
        }
    }
}