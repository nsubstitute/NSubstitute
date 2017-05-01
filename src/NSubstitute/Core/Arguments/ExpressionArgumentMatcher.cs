using System;
using System.Linq.Expressions;

namespace NSubstitute.Core.Arguments
{
    public class ExpressionArgumentMatcher<T> : IArgumentMatcher
    {
        readonly string _predicateDescription;
        private readonly Predicate<T> _predicate;

        public ExpressionArgumentMatcher(Expression<Predicate<T>> predicate)
        {
            _predicate = predicate.Compile();
            _predicateDescription = predicate.ToString();
        }

        public bool IsSatisfiedBy(object argument)
        {
            return _predicate((T)argument);
        }

        public override string ToString() { return _predicateDescription; }
    }
}