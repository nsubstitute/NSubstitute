using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NSubstitute.Core.Arguments
{
    public abstract class ArgumentSpecification : IArgumentSpecification
    {
        public abstract bool IsSatisfiedBy(object argument);
        private readonly Type _forType;
        public Type ForType { get { return _forType; } }
        protected ArgumentSpecification(Type forType) { _forType = forType; Action = x => { }; }
        public Action<object> Action { get; set; }
    }

    public class ArgumentIsAnythingSpecification : ArgumentSpecification
    {
        public ArgumentIsAnythingSpecification(Type forType) : base(forType) { }
        public override string ToString() { return "any " + ForType.Name; }
        public override bool IsSatisfiedBy(object argument)
        {
            if (argument == null) return true;
            var specifiedType = (ForType.IsByRef) ? ForType.GetElementType() : ForType;
            return specifiedType.IsAssignableFrom(argument.GetType());
        }
    }

    public class ArgumentEqualsSpecification : ArgumentSpecification
    {
        private readonly object _value;

        public ArgumentEqualsSpecification(object value, Type forType) : base(forType) { _value = value; }

        private bool Matches(object argument)
        {
            return EqualityComparer<object>.Default.Equals(_value, argument);
        }

        public override string ToString() { return new ArgumentFormatter().Format(_value); }
        public override bool IsSatisfiedBy(object argument) { return Matches(argument); }
    }

    public class ArgumentMatchesSpecification<T> : ArgumentSpecification
    {
        readonly string _predicateDescription;
        private readonly Predicate<T> _predicate;

        public ArgumentMatchesSpecification(Expression<Predicate<T>> predicate)
            : base(typeof(T))
        {
            _predicate = predicate.Compile();
            _predicateDescription = predicate.ToString();
        }

        public override bool IsSatisfiedBy(object argument)
        {
            if (!ArgumentIsCompatibleWithType(argument)) return false;
            try
            {
                return _predicate((T)argument);
            }
            catch
            {
                return false;
            }
        }

        private bool ArgumentIsCompatibleWithType(object argument)
        {
            return argument == null ? TypeCanBeNull() : ForType.IsAssignableFrom(argument.GetType());
        }

        private bool TypeCanBeNull() { return !ForType.IsValueType; }
        public override string ToString() { return _predicateDescription; }
    }

    public class ArrayContentsArgumentSpecification : ArgumentSpecification
    {
        private readonly IEnumerable<IArgumentSpecification> _argumentSpecifications;

        public ArrayContentsArgumentSpecification(IEnumerable<IArgumentSpecification> argumentSpecifications, Type forType)
            : base(forType)
        {
            _argumentSpecifications = argumentSpecifications;
        }

        public override bool IsSatisfiedBy(object argument)
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

        public override string ToString()
        {
            return string.Join(", ", _argumentSpecifications.Select(x => x.ToString()).ToArray());
        }
    }
}