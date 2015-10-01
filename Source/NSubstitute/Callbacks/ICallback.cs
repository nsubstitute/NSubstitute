using System;
using NSubstitute.Core;

namespace NSubstitute.Callbacks
{
    public interface ICallback
    {
        /// <summary>
        /// Throw exception returned by function once when called in a chain of callbacks.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="throwThis">The throw this.</param>
        /// <returns></returns>
        ICallback ThenThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception;

        /// <summary>
        /// Throw this exception once when called in a chain of callbacks.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        ICallback ThenThrow<TException>(TException exception) where TException : Exception;

        /// <summary>
        /// Throw exception returned by function always when callback is called.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="throwThis">The throw this.</param>
        /// <returns></returns>
        ICallback AlwaysThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception;

        /// <summary>
        /// Throw this exception always when callback is called.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        ICallback AlwaysThrow<TException>(TException exception) where TException : Exception;

        /// <summary>
        /// Perform this action always when callback is called.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
        ICallback Always(Action<CallInfo> doThis);

        /// <summary>
        /// Perform this action once in chain of called callbacks.
        /// </summary>
        /// <param name="doThis"></param>
        /// <returns></returns>
        ICallback Then(Action<CallInfo> doThis);
    }
}