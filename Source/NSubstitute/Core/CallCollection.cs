using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallCollection : ICallCollection, IReceivedCalls
    {
        ConcurrentQueue<CallWrapper> _callWrappers = new ConcurrentQueue<CallWrapper>();

        public void Add(ICall call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));
            _callWrappers.Enqueue(new CallWrapper(call));
        }

        public void Delete(ICall call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            var callWrapper = _callWrappers.FirstOrDefault(w => !w.IsDeleted && call.Equals(w.Call));
            if (callWrapper == null)
            {
                throw new SubstituteInternalException("CallCollection.Delete - collection doesn't contain the call");
            }

            callWrapper.Delete();
        }

        public IEnumerable<ICall> AllCalls()
        {
            return _callWrappers.ToArray()
                .Where(w => !w.IsDeleted)
                .Select(w => w.Call);
        }

        public void Clear()
        {
            //Queue doesn't have a Clear method.
            _callWrappers = new ConcurrentQueue<CallWrapper>();
        }

        /// <summary>
        /// Wrapper to track that particular entry was deleted.
        /// That is needed because concurrent collections don't have a Delete method.
        /// We null the hold value to not leak memory.
        /// </summary>
        private class CallWrapper
        {
            public ICall Call { get; private set; }
            public bool IsDeleted => Call == null;

            public CallWrapper(ICall call)
            {
                Call = call;
            }

            public void Delete()
            {
                Call = null;
            }
        }
    }
}