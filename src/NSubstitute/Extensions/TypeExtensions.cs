using System;

namespace NSubstitute.Extensions
{
    internal static class TypeExtensions
    {
#if NET40
        /// <summary>
        /// Extension method to replicate NET45 and NETSTANDARD reflection API in NET40
        /// and avoid additional compilation symbols
        /// </summary>
        internal static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif
    }
}
