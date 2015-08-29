using System;
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
        public static object ReturnsForAll<T>(this object substitute, T returnThis)
        {
            var _callRouter = SubstitutionContext.Current.GetCallRouterFor(substitute);
            _callRouter.SetReturnForType(typeof(T), new ReturnValue(returnThis));
            return substitute;
        }
    }
}