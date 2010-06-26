using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallStack : ICallStack, IReceivedCalls
    {
        Stack<ICall> _stack;

        public CallStack()
        {
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

        public IEnumerable<ICall> FindMatchingCalls(ICallSpecification callSpecification)
        {
            return _stack.Where(x => callSpecification.IsSatisfiedBy(x));
        }

        public IEnumerable<ICall> AllCalls()
        {
            foreach (var call in _stack)
            {
                yield return call;
            }
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}