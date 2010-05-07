using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public abstract class ArgumentSpecification : IArgumentSpecification
    {
        private Predicate<object> _matchingCriteria;

        public ArgumentSpecification(Predicate<object> matchingCriteria)
        {
            _matchingCriteria = matchingCriteria;
        }

        public bool IsSatisfiedBy(object argument)
        {
            return _matchingCriteria(argument);
        }
    }

    public class ArgumentIsAnythingSpecification : ArgumentSpecification
    {
        public ArgumentIsAnythingSpecification() : base(arg => true) { }
    }

    public class ArgumentEqualsSpecification : ArgumentSpecification
    {
        public ArgumentEqualsSpecification(object value) : base(arg => EqualityComparer<object>.Default.Equals(value, arg)) { }
    }

    public class ArgumentMatchesSpecification : ArgumentSpecification
    {
        public ArgumentMatchesSpecification(Predicate<object> matchingCriteria) : base(matchingCriteria) { }
    }


}