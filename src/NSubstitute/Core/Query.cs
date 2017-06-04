using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class Query : IQueryResults
    {
        readonly List<CallSpecAndTarget> _querySpec = new List<CallSpecAndTarget>();
        readonly HashSet<ICall> _matchingCalls = new HashSet<ICall>(new CallSequenceNumberComparer());
        
        public void Add(ICallSpecification callSpecification, object target)
        {
            _querySpec.Add(new CallSpecAndTarget(callSpecification, target));
            var allMatchingCallsOnTarget = target.ReceivedCalls().Where(callSpecification.IsSatisfiedBy);
            foreach (var matchingCall in allMatchingCallsOnTarget) { _matchingCalls.Add(matchingCall); }
        }

        public IQueryResults Result()
        {
            return this;
        }

        IEnumerable<ICall> IQueryResults.MatchingCallsInOrder()
        {
            return _matchingCalls.OrderBy(x => x.GetSequenceNumber());
        }

        IEnumerable<CallSpecAndTarget> IQueryResults.QuerySpecification()
        {
            return _querySpec.Select(x => x);
        }

        private class CallSequenceNumberComparer : IEqualityComparer<ICall>
        {
            public bool Equals(ICall x, ICall y)
            {
                return x.GetSequenceNumber() == y.GetSequenceNumber();
            }

            public int GetHashCode(ICall obj)
            {
                return obj.GetSequenceNumber().GetHashCode();
            }
        }
    }
}