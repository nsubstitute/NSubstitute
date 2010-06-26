using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        public override string ToString()
        {
            return "<any " + ForType.Name + ">";
        }
    }

    public class ArgumentEqualsSpecification : ArgumentSpecification
    {
        private readonly object _value;

        public ArgumentEqualsSpecification(object value, Type forType) : base(arg => EqualityComparer<object>.Default.Equals(value, arg), forType)
        {
            _value = value;
        }

        public override string ToString()
        {
            return new ArgumentFormatter().Format(_value); 
        }
    }

    public class ArgumentMatchesSpecification : ArgumentSpecification
    {
        private readonly Expression<Predicate<object>> _matchingCriteria;

        public ArgumentMatchesSpecification(Expression<Predicate<object>> matchingCriteria, Type forType) : base(matchingCriteria.Compile(), forType)
        {
            _matchingCriteria = matchingCriteria;
        }

        public override string ToString()
        {
            return _matchingCriteria.Body.ToString();
        }
    }


}