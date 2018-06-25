using System;
using System.Collections.Concurrent;

namespace NSubstitute.Core
{
    public class CallBaseConfiguration : ICallBaseConfiguration
    {
        private ConcurrentQueue<ICallSpecification> Exclusions { get; } = new ConcurrentQueue<ICallSpecification>();

        /// <inheritdoc />
        public bool CallBaseByDefault { get; set; }

        /// <inheritdoc />
        public void Exclude(ICallSpecification callSpecification)
        {
            if (callSpecification == null) throw new ArgumentNullException(nameof(callSpecification));

            Exclusions.Enqueue(callSpecification);
        }

        /// <inheritdoc />
        public bool ShouldCallBase(ICall call)
        {
            if (call == null) throw new ArgumentNullException(nameof(call));

            if (IsCallExplicitlyExcluded(call)) return false;

            return CallBaseByDefault;
        }

        private bool IsCallExplicitlyExcluded(ICall call)
        {
            // Use explicit foreach instead of LINQ to improve performance (avoid delegate construction).
            foreach (var callSpecification in Exclusions)
            {
                if (callSpecification.IsSatisfiedBy(call)) return true;
            }

            return false;
        }
    }
}