using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallStack : ICallStack, IReceivedCalls
    {
        readonly System.Collections.Concurrent.ConcurrentStack<ICall> _stack
            = new System.Collections.Concurrent.ConcurrentStack<ICall>();

        public void Push(ICall call)
        {
            _stack.Push(call);
        }

        public ICall Pop()
        {
            ICall call;
            if (!_stack.TryPop(out call)) throw new InvalidOperationException("Stack empty");
            return call;
        }

        public IEnumerable<ICall> AllCalls()
        {
            return _stack.ToArray().Reverse();
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}