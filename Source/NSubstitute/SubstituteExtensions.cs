using System;

namespace NSubstitute
{
    public static class SubstituteExtensions
    {
        public static void Return<T>(this T valuebeingextended, T returnThis, params T[] returnThese)
        {
            SubstitutionContext.Current.LastInvocationShouldReturn(returnThis);
        }
    }
}