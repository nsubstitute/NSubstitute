using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NSubstitute.Core.Arguments
{
    public abstract class ArgumentSpecification : IArgumentSpecification
    {
        protected readonly Predicate<object> _matchingCriteria;
        public Type ForType { get; private set; }

        protected ArgumentSpecification(Predicate<object> matchingCriteria, Type forType)
        {
            _matchingCriteria = matchingCriteria;
            ForType = forType;
        }

        virtual public bool IsSatisfiedBy(object argument)
        {
            return _matchingCriteria(argument);
        }
    }

    public class ArgumentIsAnythingSpecification : ArgumentSpecification
    {
        public ArgumentIsAnythingSpecification(Type forType) : base(arg => true, forType) { }
        public override string ToString()
        {
            return "any " + ForType.Name;
        }
    }

    public class ArgumentEqualsSpecification : IArgumentSpecification
    {
        private readonly object _value;
        private Type _forType;

        public Type ForType
        {
            get { return _forType; }
        }

        public ArgumentEqualsSpecification(object value, Type forType)
        {
            _value = value;
            _forType = forType;
        }

        private bool Matches(object argument)
        {
            return EqualityComparer<object>.Default.Equals(_value, argument);
        }

        public override string ToString()
        {
            return new ArgumentFormatter().Format(_value); 
        }

        public bool IsSatisfiedBy(object argument)
        {
            return Matches(argument);
        }
    }

    public class ArgumentMatchesSpecification<T> : ArgumentSpecification
    {
        readonly string _predicateDescription;

        public ArgumentMatchesSpecification(Expression<Predicate<T>> predicate)
            : base(arg => predicate.Compile().Invoke((T)arg), typeof(T))
        {
            _predicateDescription = predicate.ToString();
        }

        public override bool IsSatisfiedBy(object argument)
        {
            if (argument == null)
            {
                var typeCantBeNull = ForType.IsValueType;
                if (typeCantBeNull)
                    return false;
            }
            else
            {
                var argumentIsCompatibleWithType = ForType.IsAssignableFrom(argument.GetType());
                if (!argumentIsCompatibleWithType)
                {
                    return false;
                }
            }
            return _matchingCriteria(argument);
        }

        public override string ToString()
        {
            return _predicateDescription;
        }
    }

    public class ArrayContentsArgumentSpecification : IArgumentSpecification
    {
        private readonly IEnumerable<IArgumentSpecification> _argumentSpecifications;
        private readonly Type _forType;

        public ArrayContentsArgumentSpecification(IEnumerable<IArgumentSpecification> argumentSpecifications, Type forType)
        {
            _argumentSpecifications = argumentSpecifications;
            _forType = forType;
        }

        public bool IsSatisfiedBy(object argument)
        {
            if (argument != null)
            {
                var argumentArray = (IEnumerable<object>)argument;
                if (argumentArray.Count() == _argumentSpecifications.Count())
                {
                    return _argumentSpecifications.Select((value, index) => value.IsSatisfiedBy(argumentArray.ElementAt(index))).All(x => x); 
                }
            }
            return false;
        }

        public Type ForType
        {
            get { return _forType; }
        }

        public override string ToString()
        {
            return string.Join(", ", _argumentSpecifications.Select(x => x.ToString()).ToArray());
        }
    }
}