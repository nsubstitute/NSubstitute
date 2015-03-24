﻿using System;
using NSubstitute.Core;

namespace NSubstitute.ExceptionExtensions
{
    public static class SubstituteExceptionExtensions
    { 
        /// <summary>
        /// Throws an exception when method is called
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="ex">Exception to throw</param>
        /// <returns></returns>
        public static ConfiguredCall Throws<T>(this T substitute, Exception ex)
        {            
            return substitute.Returns(_ => { throw ex; });
        }

        /// <summary>
        /// Throws an exception generated by the specified function when called.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="createException">Func creating exception object</param>
        /// <returns></returns>
        public static ConfiguredCall Throws<T>(this T substitute, Func<CallInfo, Exception> createException)
        {
            return substitute.Returns(ci => { throw createException(ci); });
        }

        /// <summary>
        /// Throws an exception when method is called regardless of method's parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="ex">Exception to throw</param>
        /// <returns></returns>
        public static ConfiguredCall ThrowsForAnyArgs<T>(this T substitute, Exception ex)
        {
            return substitute.ReturnsForAnyArgs(_ => { throw ex; });
        }

        /// <summary>
        /// Throws an exception generated by the specified function when method is called regardless of method's parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="createException">Func creating exception object</param>
        /// <returns></returns>
        public static ConfiguredCall ThrowsForAnyArgs<T>(this T substitute, Func<CallInfo, Exception> createException)
        {
            return substitute.ReturnsForAnyArgs(ci => { throw createException(ci); });
        }
    }
}
