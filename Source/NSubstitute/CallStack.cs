using System.Collections.Generic;

namespace NSubstitute
{
    public class CallStack : ICallStack
    {
        Stack<IInvocation> _stack;

        public CallStack()
        {
            _stack = new Stack<IInvocation>();
        }

        public void Push(IInvocation invocation)
        {
            _stack.Push(invocation);
        }

        public IInvocation Pop()
        {
            return _stack.Pop();
        }
    }
}