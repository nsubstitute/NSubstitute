namespace NSubstitute
{
    public interface IInvocationHandler
    {
        void LastInvocationShouldReturn<T>(T valueToReturn);
    }
}