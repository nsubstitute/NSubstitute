namespace NSubstitute
{
    public class SubstitutionFactory : ISubstitutionFactory
    {
        public static ISubstitutionFactory Current { get; set; }
        public T Create<T>()
        {
            return default(T);
        }
    }
}