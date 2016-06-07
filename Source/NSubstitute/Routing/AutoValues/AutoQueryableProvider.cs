using System;
using System.Linq;
using System.Text;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoQueryableProvider : IAutoValueProvider, IMaybeAutoValueProvider
    {
        public bool CanProvideValueFor(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>);
        }

        public object GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                throw new InvalidOperationException();

            Type innerType = type.GetGenericArguments()[0];

            return Array.CreateInstance(innerType, 0).AsQueryable();
        }

        Maybe<object> IMaybeAutoValueProvider.GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetValue(type));
        }
    }
}
