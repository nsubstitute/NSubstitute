using System;
using System.Reflection;

namespace NSubstitute.Core
{
    public class DefaultForType : IDefaultForType
    {
        private static readonly object BoxedBoolean = default(bool);
        private static readonly object BoxedInt = default(int);
        private static readonly object BoxedLong = default(long);
        private static readonly object BoxedDouble = default(double);

        public object? GetDefaultFor(Type type)
        {
            if (IsVoid(type))
            {
                return null;
            }

            if (type.GetTypeInfo().IsValueType)
            {
                return DefaultInstanceOfValueType(type);
            }

            return null;
        }

        private bool IsVoid(Type returnType) => returnType == typeof(void);

        private object DefaultInstanceOfValueType(Type returnType)
        {
            // Performance optimization for the most popular types.
            if (returnType == typeof(bool))
            {
                return BoxedBoolean;
            }
            if (returnType == typeof(int))
            {
                return BoxedInt;
            }
            if (returnType == typeof(long))
            {
                return BoxedLong;
            }
            if (returnType == typeof(double))
            {
                return BoxedDouble;
            }

            return Activator.CreateInstance(returnType)!;
        }
    }
}