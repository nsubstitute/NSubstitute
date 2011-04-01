using System;
using System.Linq.Expressions;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;

namespace NSubstitute
{
    public static class Arg
    {
        public static T Any<T>()
        {
            return EnqueueArgumentSpec<T>(new ArgumentIsAnythingSpecification(typeof(T)));
        }

        public static T Is<T>(T value)
        {
            return EnqueueArgumentSpec<T>(new ArgumentEqualsSpecification(value, typeof(T)));
        }

        public static T Is<T>(Expression<Predicate<T>> predicate)
        {
            return EnqueueArgumentSpec<T>(new ArgumentMatchesSpecification<T>(predicate));
        }

        private static T EnqueueArgumentSpec<T>(IArgumentSpecification argumentSpecification)
        {
            SubstitutionContext.Current.EnqueueArgumentSpecification(argumentSpecification);
            return default(T);
        }
    }
}