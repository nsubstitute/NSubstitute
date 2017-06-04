using System;
using System.Reflection;

namespace NSubstitute.Core
{
    public class DefaultForType : IDefaultForType
    {
        public object GetDefaultFor(Type type)
        {
            if (IsVoid(type)) return null;
            if (type.IsValueType()) return DefaultInstanceOfValueType(type);
            return null;
        }

        private bool IsVoid(Type returnType)
        {
            return returnType == typeof(void);
        }

        private object DefaultInstanceOfValueType(Type returnType)
        {
            return Activator.CreateInstance(returnType);
        }
    }
}