using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Combines two enumerables into a new enumerable using the given selector.
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <remarks>
        /// This implementation was sanity-checked against the 
        /// <a href="http://msmvps.com/blogs/jon_skeet/archive/2011/01/14/reimplementing-linq-to-objects-part-35-zip.aspx">Edulinq implementation</a> and
        /// <a href="http://blogs.msdn.com/b/ericlippert/archive/2009/05/07/zip-me-up.aspx">Eric Lippert's implementation</a>.
        /// </remarks>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> selector)
        {
            using (var firstEnumerator = first.GetEnumerator())
            using (var secondEnumerator = second.GetEnumerator())
            {
                while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
                {
                    yield return selector(firstEnumerator.Current, secondEnumerator.Current);
                }
            }
        }

        /// <summary>
        /// Checks if the instance can be used when a <paramref name="type"/> is expected.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCompatibleWith(this object instance, Type type)
        {
            var requiredType = (type.IsByRef) ? type.GetElementType() : type;
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
            return !type.IsValueType() || Nullable.GetUnderlyingType(type) != null;
        }

        public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.MoveNext() ? Maybe.Just(enumerator.Current) : Maybe.Nothing<T>();
        }
    }
}