namespace NSubstituteSpike
{
    public interface ISubstitutionFactory
    {
        T Create<T>();
    }
}