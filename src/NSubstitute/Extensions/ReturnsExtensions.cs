using System.Threading.Tasks;
using NSubstitute.Core;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute.ReturnsExtensions
{
    public static class ReturnsExtensions
    {
        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        public static ConfiguredCall ReturnsNull<T>(this T value) where T : class =>
            value.Returns(default(T));

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this T value) where T : class =>
            value.ReturnsForAnyArgs(default(T));

        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        public static ConfiguredCall ReturnsNull<T>(this T? value) where T : struct =>
            value.Returns(default(T?));

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this T? value) where T : struct =>
            value.ReturnsForAnyArgs(default(T?));

        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        public static ConfiguredCall ReturnsNull<T>(this Task<T> value) where T : class =>
            value.Returns(default(T));

        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        public static ConfiguredCall ReturnsNull<T>(this ValueTask<T> value) where T : class =>
            value.Returns(default(T));

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this Task<T> value) where T : class =>
            value.ReturnsForAnyArgs(default(T));

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this ValueTask<T> value) where T : class =>
            value.ReturnsForAnyArgs(default(T));

        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        public static ConfiguredCall ReturnsNull<T>(this Task<T?> value) where T : struct =>
            value.Returns(default(T?));

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this Task<T?> value) where T : struct =>
            value.ReturnsForAnyArgs(default(T?));

        /// <summary>
        /// Set null as returned value for this call.
        /// </summary>
        public static ConfiguredCall ReturnsNull<T>(this ValueTask<T?> value) where T : struct =>
            value.Returns(default(T?));

        /// <summary>
        /// Set null as returned value for this call made with any arguments.
        /// </summary>
        public static ConfiguredCall ReturnsNullForAnyArgs<T>(this ValueTask<T?> value) where T : struct =>
            value.ReturnsForAnyArgs(default(T?));
    }
}
