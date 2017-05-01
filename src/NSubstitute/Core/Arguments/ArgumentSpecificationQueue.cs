using System;

namespace NSubstitute.Core.Arguments
{
    public class ArgumentSpecificationQueue
    {
        private readonly ISubstitutionContext _substitutionContext;

        public ArgumentSpecificationQueue(ISubstitutionContext substitutionContext)
        {
            _substitutionContext = substitutionContext;
        }

        public T EnqueueSpecFor<T>(IArgumentMatcher matcher, Action<object> action)
        {
            return EnqueueSpecFor<T>(new ArgumentSpecification(typeof(T), matcher, action));
        }

        public T EnqueueSpecFor<T>(IArgumentMatcher<T> matcher)
        {
            return EnqueueSpecFor<T>(new ArgumentSpecification(typeof(T), new GenericToNonGenericArgumentMatcher<T>(matcher)));
        }

        public T EnqueueSpecFor<T>(IArgumentMatcher matcher)
        {
            return EnqueueSpecFor<T>(new ArgumentSpecification(typeof(T), matcher));
        }

        private T EnqueueSpecFor<T>(IArgumentSpecification argumentSpecification)
        {
            _substitutionContext.EnqueueArgumentSpecification(argumentSpecification);
            return default(T);
        }

        private class GenericToNonGenericArgumentMatcher<T> : IArgumentMatcher, IDescribeNonMatches
        {
            private readonly IArgumentMatcher<T> _genericMatcher;

            public GenericToNonGenericArgumentMatcher(IArgumentMatcher<T> genericMatcher)
            {
                _genericMatcher = genericMatcher;
            }

            public bool IsSatisfiedBy(object argument)
            {
                return _genericMatcher.IsSatisfiedBy((T)argument);
            }

            public override string ToString()
            {
                return _genericMatcher.ToString();
            }

            public string DescribeFor(object argument)
            {
                var describable = _genericMatcher as IDescribeNonMatches;
                return describable == null ? string.Empty : describable.DescribeFor(argument);
            }
        }
    }
}