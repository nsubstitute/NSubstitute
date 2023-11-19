using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class SuppliedArgumentSpecifications : ISuppliedArgumentSpecifications
    {
        private readonly IArgumentSpecificationCompatibilityTester _argSpecCompatibilityTester;
        private readonly Queue<IArgumentSpecification> _queue;
        private IReadOnlyCollection<IArgumentSpecification> AllSpecifications { get; }

        public SuppliedArgumentSpecifications(IArgumentSpecificationCompatibilityTester argSpecCompatibilityTester, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _argSpecCompatibilityTester = argSpecCompatibilityTester;
            AllSpecifications = argumentSpecifications.ToArray();
            _queue = new Queue<IArgumentSpecification>(AllSpecifications);
        }


        public bool AnyFor(object? argument, Type argumentType)
        {
            return AllSpecifications.Any(x => _argSpecCompatibilityTester.IsSpecificationCompatible(x, argument, argumentType));
        }

        public bool IsNextFor(object? argument, Type argumentType)
        {
            if (_queue.Count == 0)
            {
                return false;
            }

            var nextArgSpec = _queue.Peek();
            return _argSpecCompatibilityTester.IsSpecificationCompatible(nextArgSpec, argument, argumentType);
        }

        public IArgumentSpecification Dequeue() => _queue.Dequeue();

        public IEnumerable<IArgumentSpecification> DequeueRemaining()
        {
            var result = _queue.ToArray();
            _queue.Clear();
            return result;
        }
    }
}