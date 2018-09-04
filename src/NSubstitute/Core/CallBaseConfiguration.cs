using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallBaseConfiguration : ICallBaseConfiguration
    {
        private ConcurrentQueue<CallBaseRule> Rules { get; } = new ConcurrentQueue<CallBaseRule>();

        /// <inheritdoc />
        public bool CallBaseByDefault { get; set; }

        /// <inheritdoc />
        public void Exclude(ICallSpecification callSpecification)
        {
            if (callSpecification == null) throw new ArgumentNullException(nameof(callSpecification));

            Rules.Enqueue(new CallBaseRule(callSpecification, callBase: false));
        }

        /// <inheritdoc />
        public void Include(ICallSpecification callSpecification)
        {
            if (callSpecification == null) throw new ArgumentNullException(nameof(callSpecification));

            Rules.Enqueue(new CallBaseRule(callSpecification, callBase: true));
        }

        /// <inheritdoc />
        public bool ShouldCallBase(ICall call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            if (TryGetExplicitConfiguration(call, out bool callBase))
            {
                return callBase;
            }

            return CallBaseByDefault;
        }

        private bool TryGetExplicitConfiguration(ICall call, out bool callBase)
        {
            // Use explicit foreach instead of LINQ to improve performance (avoid delegate construction).
            foreach (var rule in Rules.Reverse())
            {
                if (rule.IsSatisfiedBy(call))
                {
                    callBase = rule.CallBase;
                    return true;
                }
            }

            callBase = false;
            return false;
        }

        private struct CallBaseRule
        {
            public ICallSpecification CallSpecification { get; }
            public bool CallBase { get; }

            public CallBaseRule(ICallSpecification callSpecification, bool callBase)
            {
                CallSpecification = callSpecification;
                CallBase = callBase;
            }

            public bool IsSatisfiedBy(ICall call) => CallSpecification.IsSatisfiedBy(call);
        }
    }
}