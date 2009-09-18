namespace NSubstitute
{
    public interface ISubstitute
    {
        void LastCallShouldReturn<T>(T valueToReturn);
    }
}