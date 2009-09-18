namespace NSubstitute
{
    public interface ICallStack
    {
        void Push(IInvocation invocation);
        IInvocation Pop();
    }
}