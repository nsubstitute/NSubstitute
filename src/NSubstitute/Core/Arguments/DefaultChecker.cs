using System;
using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public class DefaultChecker : IDefaultChecker
    {
        private readonly IDefaultForType _defaultForType;

        public DefaultChecker(IDefaultForType defaultForType)
        {
            _defaultForType = defaultForType;
        }

        public bool IsDefault(object? value, Type forType)
        {
            return EqualityComparer<object>.Default.Equals(value, _defaultForType.GetDefaultFor(forType));
        }
    }
}