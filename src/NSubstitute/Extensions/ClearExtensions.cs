using System;
using NSubstitute.Core;

namespace NSubstitute.ClearExtensions
{
    public static class ClearExtensions
    {
        /// <summary>
        /// Clears received calls, configured return values and/or call actions for this substitute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="substitute"></param>
        /// <param name="options">Specifies what to clear on the substitute. Can be combined with <code>|</code> to 
        /// clear multiple aspects at once.</param>
        /// <remarks>
        /// </remarks>
        public static void ClearSubstitute<T>(this T substitute, ClearOptions options = ClearOptions.All) where T : class
        {
            var context = SubstitutionContext.Current;
            var router = context.GetCallRouterFor(substitute);
            router.Clear(options);
        }
    }
}