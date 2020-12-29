using System;
using System.Linq;
using NSubstitute.Core;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute
{
    public static partial class SubstituteExtensions
    {
        /// <summary>
        /// Set a return value for this call.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        public static ConfiguredCall Returns<T>(this T value, T returnThis, params T[] returnThese) =>
            ConfigureReturn(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);

        /// <summary>
        /// Set a return value for this call, calculated by the provided function.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        public static ConfiguredCall Returns<T>(this T value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese) =>
            ConfigureReturn(MatchArgs.AsSpecifiedInCall, returnThis, returnThese);

        /// <summary>
        /// Set a return value for this call made with any arguments.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Value to return</param>
        /// <param name="returnThese">Optionally return these values next</param>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this T value, T returnThis, params T[] returnThese) =>
            ConfigureReturn(MatchArgs.Any, returnThis, returnThese);

        /// <summary>
        /// Set a return value for this call made with any arguments, calculated by the provided function.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="returnThis">Function to calculate the return value</param>
        /// <param name="returnThese">Optionally use these functions next</param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsForAnyArgs<T>(this T value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese) =>
            ConfigureReturn(MatchArgs.Any, returnThis, returnThese);

#nullable restore
        private static ConfiguredCall ConfigureReturn<T>(MatchArgs matchArgs, T? returnThis, T?[]? returnThese)
        {
            IReturn returnValue;
            if (returnThese == null || returnThese.Length == 0)
            {
                returnValue = new ReturnValue(returnThis);
            }
            else
            {
                returnValue = new ReturnMultipleValues<T>(new[] { returnThis }.Concat(returnThese).ToArray());
            }
            return SubstitutionContext
                .Current
                .ThreadContext
                .LastCallShouldReturn(returnValue, matchArgs);
        }

        private static ConfiguredCall ConfigureReturn<T>(MatchArgs matchArgs, Func<CallInfo, T?> returnThis, Func<CallInfo, T?>[]? returnThese)
        {
            IReturn returnValue;
            if (returnThese == null || returnThese.Length == 0)
            {
                returnValue = new ReturnValueFromFunc<T>(returnThis);
            }
            else
            {
                returnValue = new ReturnMultipleFuncsValues<T>(new[] { returnThis }.Concat(returnThese).ToArray());
            }

            return SubstitutionContext
                .Current
                .ThreadContext
                .LastCallShouldReturn(returnValue, matchArgs);
        }
    }
}