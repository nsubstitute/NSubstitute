namespace NSubstituteSpike
{
    public class Substitute
    {
        private readonly static Substitute Instance = new Substitute();
        private Substitute() {}

        public ISubstitute LastSubstitute;

        public static SubstituteFoo For<TSubstitute>()
        {
            var substitute = new SubstituteFoo();
            ((ISubstitute) substitute).Instance = Instance;
            return substitute;
        }


        public static void LastCallShouldReturn(object o)
        {
            Instance.LastSubstitute.LastCallShouldReturn(o);
        }
    }
}