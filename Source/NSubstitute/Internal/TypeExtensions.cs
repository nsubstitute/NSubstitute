namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class TypeExtensions
    {
#if NETSTANDARD1_5
        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }
#endif

#if NETSTANDARD1_5
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }
#endif

#if NETSTANDARD1_5
        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }
#endif

#if NETSTANDARD1_5
        public static bool IsSealed(this Type type)
        {
            return type.GetTypeInfo().IsSealed;
        }
#endif

#if NETSTANDARD1_5
        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }
#endif

#if NETSTANDARD1_5
        public static bool IsSubclassOf(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsSubclassOf(otherType);
        }
#endif
    }
}
