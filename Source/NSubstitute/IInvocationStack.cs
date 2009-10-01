namespace NSubstitute
{
    public interface IInvocationStack
    {
        void Push(IInvocation invocation);
        IInvocation Pop();
    }
}