using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable RedundantUsingDirective. Used for StackForSilverlight<T>.
using System.Threading;
// ReSharper restore RedundantUsingDirective

namespace NSubstitute.Core
{
    public class CallStack : ICallStack, IReceivedCalls
    {
#if SILVERLIGHT
        readonly StackForSilverlight<ICall> _stack = new StackForSilverlight<ICall>();
#else
        readonly System.Collections.Concurrent.ConcurrentStack<ICall> _stack
            = new System.Collections.Concurrent.ConcurrentStack<ICall>();
#endif

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

#if SILVERLIGHT
        private class StackForSilverlight<T>
        {
            readonly object _lock = new object();
            readonly Stack<T> _stack = new Stack<T>();

            public void Clear() { LockedAction(() => _stack.Clear()); }

            public void Push(T item) { LockedAction(() => _stack.Push(item)); }

            public bool TryPop(out T item)
            {
                T poppedItem = default(T);
                LockedAction(() =>  poppedItem = _stack.Pop());
                item = poppedItem;
                return true;
            }

            public T[] ToArray() { LockedAction(() => _stack.ToArray()); }

            private void LockedAction(Action action)
            {
                Monitor.Enter(_lock);
                try { action(); }
                finally { Monitor.Exit(_lock); }
            }
        }
#endif
    }
}