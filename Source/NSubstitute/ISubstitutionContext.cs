namespace NSubstitute
{
    public interface ISubstitutionContext
    {
        void LastCallShouldReturn(object value);
    }
}