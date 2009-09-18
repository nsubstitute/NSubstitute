namespace NSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        private ISubstitute lastSubstitute;

        public static void SetCurrent(ISubstitutionContext context)
        {
            Current = context;
        }

        public static ISubstitutionContext Current { get; private set; }
        
        public void LastCallShouldReturn<T>(T value)
        {
            lastSubstitute.LastCallShouldReturn(value);
        }

        public void LastSubstitute(ISubstitute substitute)
        {
            lastSubstitute = substitute;
        }
    }
}