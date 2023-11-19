using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    public class CallCollection : ICallCollection
    {
        private ConcurrentQueue<IReceivedCallEntry> _callEntries = new();

        public void Add(ICall call)
        {
            IReceivedCallEntry callEntry;
            if (call is IReceivedCallEntry callAsEntry && callAsEntry.TryTakeEntryOwnership())
            {
                callEntry = callAsEntry;
            }
            else
            {
                callEntry = new ReceivedCallEntry(call);
            }

            _callEntries.Enqueue(callEntry);
        }

        public void Delete(ICall call)
        {
            var callWrapper = _callEntries.FirstOrDefault(e => !e.IsSkipped && call.Equals(e.Call));
            if (callWrapper == null)
            {
                throw new SubstituteInternalException("CallCollection.Delete - collection doesn't contain the call");
            }

            callWrapper.Skip();
        }

        public IEnumerable<ICall> AllCalls()
        {
            return _callEntries
                .Where(e => !e.IsSkipped)
                .Select(e => e.Call!)
                .ToArray();
        }

        public void Clear()
        {
            // Queue doesn't have a Clear method.
            _callEntries = new ConcurrentQueue<IReceivedCallEntry>();
        }

        /// <summary>
        /// Performance optimization. Allows to mark call as deleted without allocating extra wrapper.
        /// To play safely, we track ownership, so object can be re-used only once.
        /// </summary>
        internal interface IReceivedCallEntry
        {
            ICall? Call { get; }
            [MemberNotNullWhen(false, nameof(Call))]
            bool IsSkipped { get; }
            void Skip();
            bool TryTakeEntryOwnership();
        }

        /// <summary>
        /// Wrapper to track that particular entry was deleted.
        /// That is needed because concurrent collections don't have a Delete method.
        /// Notice, in most cases the original <see cref="Call"/> instance will be used as a wrapper itself.
        /// </summary>
        private class ReceivedCallEntry : IReceivedCallEntry
        {
            public ICall? Call { get; private set; }

            [MemberNotNullWhen(false, nameof(Call))]
            public bool IsSkipped => Call == null;

            public void Skip() => Call = null; // Null the hold value to reclaim a bit of memory.

            public bool TryTakeEntryOwnership() =>
                throw new SubstituteInternalException("Ownership is never expected to be obtained for this entry.");

            public ReceivedCallEntry(ICall call)
            {
                Call = call;
            }
        }
    }
}