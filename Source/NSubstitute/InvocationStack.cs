using System.Collections.Generic;

namespace NSubstitute
{
    public class InvocationStack : IInvocationStack
    {
        Stack<IInvocation> _stack;

        public InvocationStack()
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