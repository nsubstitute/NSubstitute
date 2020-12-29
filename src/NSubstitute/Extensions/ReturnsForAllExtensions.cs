using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

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
            if (substitute == null) throw new NullSubstituteReferenceException();

            var callRouter = SubstitutionContext.Current.GetCallRouterFor(substitute);
            callRouter.SetReturnForType(typeof(T), new ReturnValue(returnThis));
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
            if (substitute == null) throw new NullSubstituteReferenceException();

            var callRouter = SubstitutionContext.Current.GetCallRouterFor(substitute);
            callRouter.SetReturnForType(typeof(T), new ReturnValueFromFunc<T>(returnThis));
        }
    }
}