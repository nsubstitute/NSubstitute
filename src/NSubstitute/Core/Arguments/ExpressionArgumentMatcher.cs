using System;
using System.Linq.Expressions;

namespace NSubstitute.Core.Arguments
{
    public class ExpressionArgumentMatcher<T> : IArgumentMatcher
    {
        private readonly string _predicateDescription;
        private readonly Predicate<T?> _predicate;

        public ExpressionArgumentMatcher(Expression<Predicate<T?>> predicate)
        {
            _predicate = predicate.Compile();
            _predicateDescription = predicate.ToString();
        }

        public bool IsSatisfiedBy(object? argument) => _predicate((T?)argument!);

        public override string ToString() => _predicateDescription;
    }
}