#if (NET4 || NET45 || NETSTANDARD1_3)
using System.Threading.Tasks;
#endif
using NSubstitute.Core;

namespace NSubstitute.ReturnsExtensions
{
    public static class ReturnsExtensions
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

#if (NET4 || NET45 || NETSTANDARD1_3)

        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsNull<T>(this Task<T> value) where T : class
        {
            return value.Returns(i => SubstituteExtensions.CompletedTask<T>(null));
        }

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this Task<T> value) where T : class
        {
            return value.ReturnsForAnyArgs(i => SubstituteExtensions.CompletedTask<T>(null));
        }
#endif
    }
}
