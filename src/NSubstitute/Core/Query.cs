using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class Query : IQuery, IQueryResults
    {
        private readonly List<CallSpecAndTarget> _querySpec = new();
        private readonly HashSet<ICall> _matchingCalls = new(new CallSequenceNumberComparer());
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public Query(ICallSpecificationFactory callSpecificationFactory)
        {
            _callSpecificationFactory = callSpecificationFactory;
        }

        public void RegisterCall(ICall call)
        {
            var target = call.Target();
            var callSpecification = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);

            _querySpec.Add(new CallSpecAndTarget(callSpecification, target));

            var allMatchingCallsOnTarget = target.ReceivedCalls().Where(callSpecification.IsSatisfiedBy);
            _matchingCalls.UnionWith(allMatchingCallsOnTarget);
        }

        public IQueryResults Result() => this;

        IEnumerable<ICall> IQueryResults.MatchingCallsInOrder() => _matchingCalls.OrderBy(x => x.GetSequenceNumber());

        IEnumerable<CallSpecAndTarget> IQueryResults.QuerySpecification() => _querySpec.Select(x => x);

        private class CallSequenceNumberComparer : IEqualityComparer<ICall>
        {
            public bool Equals(ICall? x, ICall? y) => x?.GetSequenceNumber() == y?.GetSequenceNumber();

            public int GetHashCode(ICall obj) => obj.GetSequenceNumber().GetHashCode();
        }
    }
}