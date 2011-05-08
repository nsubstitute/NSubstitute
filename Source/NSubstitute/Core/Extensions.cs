using System;
using System.Collections.Generic;

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
    }
}