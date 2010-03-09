using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public class InvocationStack : IInvocationStack
    {
        private readonly IInvocationMatcher _invocationMatcher;
        Stack<IInvocation> _stack;

        public InvocationStack(IInvocationMatcher invocationMatcher)
        {
            _invocationMatcher = invocationMatcher;
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

        public void ThrowIfCallNotFound(IInvocation invocation)
        {
            if (_stack.Any(receivedInvocation => _invocationMatcher.IsMatch(receivedInvocation, invocation))) return;
            throw new CallNotReceivedException();
        }
    }
}