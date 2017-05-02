using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public class PropertyEqualsArgumentMatcher : IArgumentMatcher
    {
        private readonly object _value;

        public PropertyEqualsArgumentMatcher(object value)
        {
            _value = value;
        }

        public bool IsSatisfiedBy(object argument)
        {
            return _value.GetType() == argument.GetType() && Equals(_value, argument, _value.GetType());
        }

        private static bool Equals(object obj1, object obj2, Type typeToCompareOn)
        {
            return typeToCompareOn
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .All(p => PropertiesEqual(p, obj1, obj2));
        }

        private static bool PropertiesEqual(PropertyInfo property, object obj1, object obj2)
        {
            var value1 = property.GetValue(obj1, null);
            var value2 = property.GetValue(obj2, null);

            return value1 == null && value2 == null
                   || (value1 != null && value1.Equals(value2));
        }
    }
}
