using System.Collections.Generic;

namespace NSubstitute.Core.Arguments
{
    public class EqualArgumentMatcher : IArgumentMatcher
    {
        private readonly object _value;
        public EqualArgumentMatcher(object value) { _value = value; }
        private bool Matches(object argument)
        {
            return EqualityComparer<object>.Default.Equals(_value, argument);
        }
        public override string ToString() { return new ArgumentFormatter().Format(_value); }
        public bool IsSatisfiedBy(object argument) { return Matches(argument); }
    }
}