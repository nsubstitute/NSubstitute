namespace NSubstitute
{
    public class SubstitutionContext : ISubstitutionContext
    {
        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        private ISubstitute lastSubstitute;

        public static void SetCurrent(ISubstitutionContext context)
        {
            Current = context;
        }

        public static ISubstitutionContext Current { get; private set; }
        
        public void LastCallShouldReturn<T>(T value)
        {            
            if (lastSubstitute == null) throw new SubstitutionException();
            lastSubstitute.LastCallShouldReturn(value);
        }

        public void LastSubstituteCalled(ISubstitute substitute)
        {
            lastSubstitute = substitute;
        }

    }
}