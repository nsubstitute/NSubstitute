using System;

namespace NSubstitute
{
    public class Arg
    {
        public static T Any<T>()
        {
            return Is<T>(argument => true);
        }

        public static T Is<T>(T value)
        {
            return Is<T>(argument => argument.Equals(value));
        }

        public static T Is<T>(Predicate<T> predicate)
        {
            SubstitutionContext.Current.AddArgument(predicate);
            return default(T);
        }
    }
}