namespace NSubstitute
{
    public interface ICallStack
    {
        void Push(ICall call);
        ICall Pop();
        void ThrowIfCallNotFound(ICall call);
    }
}