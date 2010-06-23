using System;
using System.Collections.Generic;

namespace NSubstitute.Core
{
    public abstract class ArgumentSpecification : IArgumentSpecification
    {
        readonly Predicate<object> _matchingCriteria;
        readonly Type _forType;

        protected ArgumentSpecification(Predicate<object> matchingCriteria, Type forType)
        {
            _matchingCriteria = matchingCriteria;
            _forType = forType;
        }

        public bool IsSatisfiedBy(object argument)
        {
            return _matchingCriteria(argument);
        }

        public Type ForType
        {
            get { return _forType; }
        }
    }

    public class ArgumentIsAnythingSpecification : ArgumentSpecification
    {
        public ArgumentIsAnythingSpecification(Type forType) : base(arg => true, forType) { }
    }

    public class ArgumentEqualsSpecification : ArgumentSpecification
    {
        public ArgumentEqualsSpecification(object value, Type forType) : base(arg => EqualityComparer<object>.Default.Equals(value, arg), forType) { }
    }

    public class ArgumentMatchesSpecification : ArgumentSpecification
    {
        public ArgumentMatchesSpecification(Predicate<object> matchingCriteria, Type forType) : base(matchingCriteria, forType) { }
    }


}