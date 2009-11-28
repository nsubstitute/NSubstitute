using System;
using System.Collections;
using System.Collections.Generic;

namespace NSubstitute
{
    public class ArgumentEqualityComparer : IArgumentEqualityComparer
    {
        private static readonly EqualityComparer<object> _equalityComparer = EqualityComparer<object>.Default;

        bool IEqualityComparer.Equals(object x, object y)
        {
            return _equalityComparer.Equals(x, y);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return _equalityComparer.GetHashCode(obj);
        }
    }
}