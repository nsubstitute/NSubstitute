using System;

namespace NSubstitute.Core
{
    public class DefaultForType : IDefaultForType
    {
        public object GetDefaultFor(Type type)
        {
            if (IsVoid(type)) return null;
            if (type.IsValueType) return DefaultInstanceOfValueType(type);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IObservable<>)) 
            {
                Type innerType = type.GetGenericArguments()[0];
                return Activator.CreateInstance(typeof(ReturnObservable<>).MakeGenericType(innerType));
            }

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