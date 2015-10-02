using System;
using NSubstitute.Callbacks;
using NSubstitute.Core;

namespace NSubstitute
{
    /// <summary>
    /// Perform this chain of callbacks and/or always callback when called.
    /// </summary>
    public static class Callback
    {
        /// <summary>
        /// Perform as first in chain of callback when called.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
        public static ICallback First(Action<CallInfo> doThis)
        {
            return new ConfiguredCallback().First(doThis);
        }

        /// <summary>
        /// Perform this action always when callback is called.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
		public static IConfiguredCallback Always(Action<CallInfo> doThis)
        {
            return new ConfiguredCallback().Always(doThis);
        }

        /// <summary>
        /// Throw exception returned by function as first callback in chain of callback when called.
        /// </summary>
        /// <param name="throwThis"></param>
        /// <returns></returns>
        public static ICallback FirstThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return new ConfiguredCallback().FirstThrow(throwThis);
        }

        /// <summary>
        /// Throw this exception as first callback in chain of callback when called.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ICallback FirstThrow<TException>(TException exception) where TException : Exception
        {
            return new ConfiguredCallback().FirstThrow(info => exception);
        }

        /// <summary>
        /// Throw exception returned by function always when callback is called.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="throwThis">The throw this.</param>
        /// <returns></returns>
		public static IConfiguredCallback AlwaysThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return new ConfiguredCallback().AlwaysThrow(throwThis);
        }

        /// <summary>
        /// Throw this exception always when callback is called.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
		public static IConfiguredCallback AlwaysThrow<TException>(TException exception) where TException : Exception
        {
            return new ConfiguredCallback().AlwaysThrow(info => exception);
        }
    }
}