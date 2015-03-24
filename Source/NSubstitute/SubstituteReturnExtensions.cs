using NSubstitute.Core;

namespace NSubstitute.ReturnExtensions
{
    public static class SubstituteReturnExtensions
    {
        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsNull<T>(this T value) where T : class
        {
            return value.Returns(i => null);
        }

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this T value) where T : class
        {
            return value.ReturnsForAnyArgs(i => null);
        }
    }
}
