using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core.Arguments
{
    public class SuppliedArgumentSpecifications : ISuppliedArgumentSpecifications
    {
        private readonly Queue<IArgumentSpecification> _queue;
        private readonly List<IArgumentSpecification> _list;

        public SuppliedArgumentSpecifications(IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _list = new List<IArgumentSpecification>(argumentSpecifications);
            _queue = new Queue<IArgumentSpecification>(_list);
        }

        public bool AnyFor(Type type)
        {
            return _list.Any(x => type.IsAssignableFrom(x.ForType));
        }

        public bool NextFor(Type type)
        {
            if (_queue.Count > 0)
            {
                return type.IsAssignableFrom(_queue.Peek().ForType);
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