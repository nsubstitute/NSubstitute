#if DNXCORE50
using ms=System.Reflection.TypeExtensions;
#endif

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
#if DNXCORE50
            return type.GetTypeInfo().IsClass;
#elif NET4 || NET35
            return type.IsClass;
#else
            return type.GetTypeInfo().IsClass;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsGenericType;
#elif NET4 || NET35
            return type.IsGenericType;
#else
            return type.GetTypeInfo().IsGenericType;
#endif
        }


        public static bool IsInterface(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsInterface;
#elif NET4 || NET35
            return type.IsInterface;
#else
            return type.GetTypeInfo().IsInterface;
#endif
        }


        public static bool IsSealed(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsSealed;
#elif NET4 || NET35
            return type.IsSealed;
#else
            return type.GetTypeInfo().IsSealed;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if DNXCORE50
            return type.GetTypeInfo().IsValueType;
#elif NET4 || NET35
            return type.IsValueType;
#else
            return type.GetTypeInfo().IsValueType;
#endif
        }

#if DNXCORE50
        public static bool IsSubclassOf(this Type type, Type otherType)
        {
            Type p = type; 
            if (p == otherType)
                return false; 
            while (p != null) {
                if (p == otherType)
                    return true;
                p = p.GetTypeInfo().BaseType; 
            }
            return false; 
        }
#endif
    }
}