using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        public IEnumerable<ICall> FindMatchingCalls(ICallSpecification callSpecification)
        {
            return AllCalls().Where(x => callSpecification.IsSatisfiedBy(x));
        }

        public IEnumerable<ICall> AllCalls()
        {
            return _stack.Reverse();
        }

        public void Clear()
        {
            _stack.Clear();
        }

#if SILVERLIGHT
        private class StackForSilverlight<T> : IEnumerable<T>
        {
            readonly object _lock = new object();
            readonly Stack<T> _stack = new Stack<T>();

            public IEnumerator<T> GetEnumerator()
            {
                IEnumerable<T> clone = null;
                LockedAction(() => clone = _stack.ToArray());
                return clone.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

            public void Clear() { LockedAction(() => _stack.Clear()); }

            public void Push(T item) { LockedAction(() => _stack.Push(item)); }

            public bool TryPop(out T item)
            {
                T poppedItem = default(T);
                LockedAction(() =>  poppedItem = _stack.Pop());
                item = poppedItem;
                return true;
            }

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