using System;
using System.Linq;
using System.Text;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoQueryableProvider : IAutoValueProvider
    {
        private bool CanProvideValueFor(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>);
        }

        private object GetActualValue(Type type)
        {
            Type innerType = type.GetGenericArguments()[0];

            return Array.CreateInstance(innerType, 0).AsQueryable();
        }

        public Maybe<object> GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetActualValue(type));
        }
    }
}
