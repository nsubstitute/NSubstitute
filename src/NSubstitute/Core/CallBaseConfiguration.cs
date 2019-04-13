using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NSubstitute.Core
{
    public class CallBaseConfiguration : ICallBaseConfiguration
    {
        // Even though ConcurrentStack allocates on each push, we expect that configuration happens rarely.
        // Stack allows us to perform reverse enumeration (which is the most common operation) cheaply.
        private ConcurrentStack<CallBaseRule> Rules { get; } = new ConcurrentStack<CallBaseRule>();

        /// <inheritdoc />
        public bool CallBaseByDefault { get; set; }

        /// <inheritdoc />
        public void Exclude(ICallSpecification callSpecification)
        {
            if (callSpecification == null) throw new ArgumentNullException(nameof(callSpecification));

            Rules.Push(new CallBaseRule(callSpecification, callBase: false));
        }

        /// <inheritdoc />
        public void Include(ICallSpecification callSpecification)
        {
            if (callSpecification == null) throw new ArgumentNullException(nameof(callSpecification));

            Rules.Push(new CallBaseRule(callSpecification, callBase: true));
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
            callBase = default;

            // Performance optimization, as enumerator retrieval allocates.
            if (Rules.IsEmpty)
            {
                return false;
            }

            // Use explicit foreach instead of LINQ to improve performance (avoid delegate construction).
            foreach (var rule in Rules)
            {
                if (rule.IsSatisfiedBy(call))
                {
                    callBase = rule.CallBase;
                    return true;
                }
            }

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