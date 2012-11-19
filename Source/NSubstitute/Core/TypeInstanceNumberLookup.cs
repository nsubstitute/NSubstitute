using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public class TypeInstanceNumberLookup
    {
        private readonly Dictionary<Type, List<object>> _lookup = new Dictionary<Type, List<object>>();

        public int GetInstanceNumberFor(object instance)
        {
            var type = instance.GetType();
            if (!_lookup.ContainsKey(type))
            {
                _lookup[type] = new List<object> { instance };
                return 1;
            }
            var instancesForType = _lookup[type];
            var index = instancesForType.IndexOf(instance);
            if (index >= 0) return index + 1;
            else
            {
                instancesForType.Add(instance);
                return instancesForType.Count;
            }
        }
    }
}