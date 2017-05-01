using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public class EqualsArgumentMatcher : IArgumentMatcher
    {
        readonly static ArgumentFormatter DefaultArgumentFormatter = new ArgumentFormatter();
        private readonly object _value;
        public EqualsArgumentMatcher(object value) { _value = value; }
        public override string ToString() { return DefaultArgumentFormatter.Format(_value, false); }
        public bool IsSatisfiedBy(object argument)
        {
            return EqualityComparer<object>.Default.Equals(_value, argument);
        }
    }
}