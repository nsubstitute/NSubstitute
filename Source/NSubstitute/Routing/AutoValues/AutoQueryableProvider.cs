#if NET45 || NETSTANDARD1_5
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoQueryableProvider : IAutoValueProvider
    {
        public bool CanProvideValueFor(Type type)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(IQueryable<>);
        }

        public object GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                throw new InvalidOperationException();

            Type innerType = type.GetGenericArguments()[0];

            return Array.CreateInstance(innerType, 0).AsQueryable();
        }
    }
}
#endif