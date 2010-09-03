using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class SuppliedArgumentSpecifications : ISuppliedArgumentSpecifications
    {
        private readonly Queue<IArgumentSpecification> _queue;

        public SuppliedArgumentSpecifications(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _queue = new Queue<IArgumentSpecification>(argumentSpecifications);
        }

        public bool AnyFor(Type type)
        {
            return _queue.Any(x => x.ForType == type);
        }

        public bool NextFor(Type type)
        {
            if (_queue.Count > 0)
            {
                return _queue.Peek().ForType == type;
            }
            return false;
        }

        public IArgumentSpecification Dequeue()
        {
            return _queue.Dequeue();
        }

        public IEnumerable<IArgumentSpecification> DequeueAll()
        {
            var result = _queue.ToArray();
            _queue.Clear();
            return result;
        }
    }
}