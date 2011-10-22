using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NSubstitute.Core
{
    public class CallResults : ICallResults
    {
        readonly ICallInfoFactory _callInfoFactory;
#if SILVERLIGHT
        readonly QueueForSilverlight<ResultForCallSpec> _results = new QueueForSilverlight<ResultForCallSpec>();
#else
        readonly System.Collections.Concurrent.ConcurrentQueue<ResultForCallSpec> _results 
            = new System.Collections.Concurrent.ConcurrentQueue<ResultForCallSpec>();
#endif

        public CallResults(ICallInfoFactory callInfoFactory)
        {
            _callInfoFactory = callInfoFactory;
        }

        public void SetResult(ICallSpecification callSpecification, IReturn result)
        {
            _results.Enqueue(new ResultForCallSpec(callSpecification, result));
        }

        public bool HasResultFor(ICall call)
        {
            if (ReturnsVoidFrom(call)) return false;
            return _results.Any(x => x.IsResultFor(call));
        }

        public object GetResult(ICall call)
        {
            return _results
                    .Reverse()
                    .First(x => x.IsResultFor(call))
                    .GetResult(_callInfoFactory.Create(call));
        }

        bool ReturnsVoidFrom(ICall call)
        {
            return call.GetReturnType() == typeof (void);
        }

        class ResultForCallSpec
        {
            readonly ICallSpecification _callSpecification;
            readonly IReturn _resultToReturn;

            public ResultForCallSpec(ICallSpecification callSpecification, IReturn resultToReturn)
            {
                _callSpecification = callSpecification;
                _resultToReturn = resultToReturn;
            }

            public bool IsResultFor(ICall call) { return _callSpecification.IsSatisfiedBy(call); }
            public object GetResult(CallInfo callInfo) { return _resultToReturn.ReturnFor(callInfo); }
        }

#if SILVERLIGHT
        private class QueueForSilverlight<T> : IEnumerable<T>
        {
            readonly object _lock = new object();
            readonly Queue<T> _queue = new Queue<T>();
            public IEnumerator<T> GetEnumerator()
            {
                Monitor.Enter(_lock);
                try { var clone = (IEnumerable<T>) _queue.ToArray(); return clone.GetEnumerator(); }
                finally { Monitor.Exit(_lock); }
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

            public void Enqueue(T item)
            {
                Monitor.Enter(_lock);
                try { _queue.Enqueue(item); }
                finally { Monitor.Exit(_lock); }
            }
        }
#endif
    }
}