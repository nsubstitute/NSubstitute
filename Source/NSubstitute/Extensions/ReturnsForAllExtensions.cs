﻿using System;
using NSubstitute.Core;

namespace NSubstitute.Extensions
{
    public static class ReturnsForAllExtensions
    {
        /// <summary>
        /// Configure default return value for all methods that return the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name = "substitute"></param>
        /// <param name="returnThis"></param>
        /// <returns></returns>
        public static void ReturnsForAll<T>(this object substitute, T returnThis)
        {
            var _callRouter = SubstitutionContext.Current.GetCallRouterFor(substitute);
            _callRouter.SetReturnForType(typeof(T), new ReturnValue(returnThis));
        }
        
        /// <summary>
        /// Configure default return value for all methods that return the specified type, calculated by a function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="returnThis"></param>
        /// <returns></returns>
        public static void ReturnsForAll<T>(this object substitute, Func<CallInfo, T> returnThis)
        {
            var _callRouter = SubstitutionContext.Current.GetCallRouterFor(substitute);
            _callRouter.SetReturnForType(typeof(T), new ReturnValueFromFunc<T>(returnThis));
        }
    }
}