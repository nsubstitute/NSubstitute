using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if the instance can be used when a <paramref name="type"/> is expected.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCompatibleWith(this object instance, Type type)
        {
            var requiredType = type.IsByRef ? type.GetElementType() : type;
            return instance == null ? TypeCanBeNull(requiredType) : requiredType.IsInstanceOfType(instance);
        }

        /// <summary>
        /// Join the <paramref name="strings"/> using <paramref name="seperator"/>.
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> strings, string seperator)
        {
            return string.Join(seperator, strings.ToArray());
        }

        private static bool TypeCanBeNull(Type type)
        {
            return !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items)
        {
            using (var enumerator = items.GetEnumerator())
            {
                return enumerator.MoveNext() ? Maybe.Just(enumerator.Current) : Maybe.Nothing<T>();
            }
        }
    }
}