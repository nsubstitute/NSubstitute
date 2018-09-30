using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class Query : IQuery, IQueryResults
    {
        private readonly List<CallSpecAndTarget> _querySpec = new List<CallSpecAndTarget>();
        private readonly HashSet<ICall> _matchingCalls = new HashSet<ICall>(new CallSequenceNumberComparer());
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public Query(ICallSpecificationFactory callSpecificationFactory)
        {
            _callSpecificationFactory = callSpecificationFactory ?? throw new ArgumentNullException(nameof(callSpecificationFactory));
        }
        
        public void RegisterCall(ICall call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            var target = call.Target();
            var callSpecification = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);

            _querySpec.Add(new CallSpecAndTarget(callSpecification, target));

            var allMatchingCallsOnTarget = target.ReceivedCalls().Where(callSpecification.IsSatisfiedBy);
            _matchingCalls.UnionWith(allMatchingCallsOnTarget);
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