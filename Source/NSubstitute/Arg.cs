using System;

namespace NSubstitute
{
    public class Arg
    {
        public static T Any<T>()
        {
            return EnqueueArgumentSpec<T>(new ArgumentIsAnythingSpecification());
        }

        public static T Is<T>(T value)
        {
            return EnqueueArgumentSpec<T>(new ArgumentEqualsSpecification(value));
        }

        public static T Is<T>(Predicate<T> predicate)
        {
            return EnqueueArgumentSpec<T>(new ArgumentMatchesSpecification(arg => predicate((T) arg)));
        }

        private static T EnqueueArgumentSpec<T>(IArgumentSpecification argumentSpec)
        {
            SubstitutionContext.Current.EnqueueArgumentSpecification(argumentSpec);
            return default(T);
        }
    }
}