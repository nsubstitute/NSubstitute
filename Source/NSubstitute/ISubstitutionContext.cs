namespace NSubstitute
{
    public interface ISubstitutionContext
    {
        void LastCallShouldReturn<T>(T value);
    }
}