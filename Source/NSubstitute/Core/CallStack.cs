using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

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

        public void ThrowIfCallNotFound(ICallSpecification callSpecification)
        {
            if (_stack.Any(receivedCall => callSpecification.IsSatisfiedBy(receivedCall))) return;
            throw new CallNotReceivedException();
        }
    }
}