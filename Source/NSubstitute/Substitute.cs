using System;
using NSubstitute.Core;

namespace NSubstitute
{
    public class Substitute
    {
        private static readonly Type[] NoAdditionalInterfaceTypes = new Type[0];
        private static readonly object[] NoConstructorArguments = new object[0];

        public static T For<T>() where T : class
        {
            return For<T>(NoAdditionalInterfaceTypes, NoConstructorArguments);
        }

        public static T For<T>(params Type[] additionalInterfaces) where T : class
        {
            return For<T>(additionalInterfaces, NoConstructorArguments);
        }

        public static T For<T>(params object[] constructorArguments) where T : class
        {
            return For<T>(NoAdditionalInterfaceTypes, constructorArguments);
        } 

        public static T For<T>(Type[] additionalInterfaces, params object[] constructorArguments) where T : class
        {
            var substituteFactory = SubstitutionContext.Current.GetSubstituteFactory();
            return substituteFactory.Create<T>();
        }
    }
}