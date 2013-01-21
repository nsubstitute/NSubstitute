using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Castle.Core;

namespace NSubstitute.Core.SequenceChecking
{
    public class InstanceTracker
    {
        readonly Dictionary<object, int> _instances = new Dictionary<object, int>(new ReferenceEqualityComparer());
        int counter = 0;

        public int InstanceNumber(object o)
        {
            int i;
            if (_instances.TryGetValue(o, out i)) { return i; }
            else
            {
                var next = ++counter;
                _instances.Add(o, next);
                return next;
            }
        }

        public int NumberOfInstances() { return counter; }

        private class ReferenceEqualityComparer : EqualityComparer<object>
        {
            public override bool Equals(object x, object y) { return object.ReferenceEquals(x, y); }
            public override int GetHashCode(object obj) { return RuntimeHelpers.GetHashCode(obj); }
        }
    }
}