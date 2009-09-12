using System;

namespace NSubstitute
{
    public class SubstitutionContext
    {
        public static void SetCurrent(ISubstitutionContext context)
        {
            Current = context;
        }

        public static ISubstitutionContext Current { get; private set; }
    }
}