using System;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoQueryableProvider : IAutoValueProvider
    {
        public bool CanProvideValueFor(Type type) =>
            type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>);

        public object GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                throw new InvalidOperationException();

            Type innerType = type.GetGenericArguments()[0];

            return Array.CreateInstance(innerType, 0).AsQueryable();
        }
    }
}
