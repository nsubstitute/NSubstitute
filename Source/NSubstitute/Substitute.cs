using System;
using NSubstitute.Core;

namespace NSubstitute
{
    public class Substitute
    {
        private static readonly Type[] NoAdditionalInterfaceTypes = new Type[0];

        public static T For<T>(params object[] constructorArguments) where T : class
        {
            return For<T>(NoAdditionalInterfaceTypes, constructorArguments);
        }

        public static T1 For<T1, T2>(params object[] constructorArguments)
            where T1 : class
            where T2 : class
        {
            return For<T1>(new[] { typeof(T2) }, constructorArguments);
        }

        public static T1 For<T1, T2, T3>(params object[] constructorArguments)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return For<T1>(new[] { typeof(T2), typeof(T3) }, constructorArguments);
        }

        public static T For<T>(Type[] additionalInterfaces, object[] constructorArguments) where T : class
        {
            //This should not be a generic method
            //Should check to make sure there is not more than 1 class being subbed for.
            var substituteFactory = SubstitutionContext.Current.GetSubstituteFactory();
            return substituteFactory.Create<T>(new Type[0], new object[0]);
        }
    }
}