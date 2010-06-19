using System;
using NSubstitute.Core;

namespace NSubstitute
{
    public class Substitute
    {
        public static T For<T>(params object[] constructorArguments) 
            where T : class
        {
            return (T) For(new[] {typeof(T)}, constructorArguments);
        }

        public static T1 For<T1, T2>(params object[] constructorArguments)
            where T1 : class
            where T2 : class
        {
            return (T1) For(new[] { typeof(T1), typeof(T2) }, constructorArguments);
        }

        public static T1 For<T1, T2, T3>(params object[] constructorArguments)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return (T1) For(new[] { typeof(T1), typeof(T2), typeof(T3) }, constructorArguments);
        }

        public static object For(Type[] typesToProxy, object[] constructorArguments) 
        {
            //Should check to make sure there is not more than 1 class being subbed for.
            var substituteFactory = SubstitutionContext.Current.GetSubstituteFactory();
            return substituteFactory.Create(typesToProxy, constructorArguments);
        }
    }
}