namespace NSubstitute
{
    public class InvocationHandler : IInvocationHandler
    {
        readonly IInvocationStack _invocationStack;
        readonly IInvocationResults _invocationResults;
        readonly ISubstitutionContext _context;

        public InvocationHandler(IInvocationStack invocationStack, IInvocationResults invocationResults, ISubstitutionContext context)
        {
            _invocationStack = invocationStack;
            _invocationResults = invocationResults;
            _context = context;
        }

        public void LastInvocationShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _invocationStack.Pop();
            _invocationResults.SetResult(lastCall, valueToReturn);
        }

        public object HandleInvocation(IInvocation invocation)
        {
            _invocationStack.Push(invocation);
            _context.LastInvocationHandlerInvoked(this);
            return _invocationResults.GetResult(invocation);
        }
    }
}