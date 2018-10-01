using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    internal static class Extensions
    {
        /// <summary>
        /// Checks if the instance can be used when a <paramref name="type"/> is expected.
        /// </summary>
        public static bool IsCompatibleWith(this object instance, Type type)
        {
            var requiredType = type.IsByRef ? type.GetElementType() : type;
            return instance == null ? TypeCanBeNull(requiredType) : requiredType.IsInstanceOfType(instance);
        }

        /// <summary>
        /// Join the <paramref name="strings"/> using <paramref name="seperator"/>.
        /// </summary>
        public static string Join(this IEnumerable<string> strings, string seperator)
        {
            return string.Join(seperator, strings);
        }

        private static bool TypeCanBeNull(Type type)
        {
            return !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        public static string GetNonMangledTypeName(this Type type)
        {
            var typeName = type.Name;
            if (!type.GetTypeInfo().IsGenericType)
                return typeName;

            typeName = typeName.Substring(0, typeName.IndexOf('`'));
            var genericArgTypes = type.GetGenericArguments().Select(GetNonMangledTypeName);
            return string.Format("{0}<{1}>", typeName, string.Join(", ", genericArgTypes));
        }
    }
}