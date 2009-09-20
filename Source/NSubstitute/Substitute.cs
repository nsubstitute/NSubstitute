namespace NSubstitute
{
    public class Substitute : ISubstitute
    {
        readonly ICallStack _callStack;
        readonly ICallResults _callResults;
        readonly ISubstitutionContext _context;

        public Substitute(ICallStack callStack, ICallResults callResults, ISubstitutionContext context)
        {
            _callStack = callStack;
            _callResults = callResults;
            _context = context;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _callStack.Pop();
            _callResults.SetResult(lastCall, valueToReturn);
        }

        public object MemberInvoked(IInvocation invocation)
        {
            _callStack.Push(invocation);
            _context.LastSubstituteCalled(this);
            return _callResults.GetResult(invocation);
        }

    }
}