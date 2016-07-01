namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class TypeExtensions
    {
        public static bool IsClass(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }


        public static bool IsInterface(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }


        public static bool IsSealed(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsSealed;
#else
            return type.IsSealed;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

#if NETSTANDARD1_5
        public static bool IsSubclassOf(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsSubclassOf(otherType);
        }
#endif
    }
}
