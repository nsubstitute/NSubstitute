namespace NSubstitute
{
    public class SubstitutionFactory : ISubstituteFactory
    {
        public static ISubstituteFactory Current { get; set; }
        public T Create<T>()
        {
            return default(T);
        }
    }
}