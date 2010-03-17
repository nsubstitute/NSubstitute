using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute
{
    public class CallStack : ICallStack
    {
        private readonly ICallMatcher _callMatcher;
        Stack<ICall> _stack;

        public CallStack(ICallMatcher CallMatcher)
        {
            _callMatcher = CallMatcher;
            _stack = new Stack<ICall>();
        }

        public void Push(ICall call)
        {
            _stack.Push(call);
        }

        public ICall Pop()
        {
            return _stack.Pop();
        }

        public void ThrowIfCallNotFound(ICall call)
        {
            if (_stack.Any(receivedCall => _callMatcher.IsMatch(receivedCall, call))) return;
            throw new CallNotReceivedException();
        }
    }
}