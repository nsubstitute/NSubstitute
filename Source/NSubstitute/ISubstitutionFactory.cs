namespace NSubstitute
{
    public interface ISubstitutionFactory
    {
        T Create<T>();
    }
}