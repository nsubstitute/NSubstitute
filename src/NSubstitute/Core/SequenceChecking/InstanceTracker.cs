using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NSubstitute.Core.SequenceChecking
{
    public class InstanceTracker
    {
        private readonly Dictionary<object, int> _instances = new(new ReferenceEqualityComparer());
        private int _counter = 0;

        public int InstanceNumber(object o)
        {
            if (_instances.TryGetValue(o, out var i))
            {
                return i;
            }

            var next = ++_counter;
            _instances.Add(o, next);
            return next;
        }

        public int NumberOfInstances() => _counter;

        private class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}