using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoArrayProvider : IAutoValueProvider, IMaybeAutoValueProvider
    {
        public bool CanProvideValueFor(Type type)
        {
            return type.IsArray;
        }

        public object GetValue(Type type)
        {
            var rank = type.GetArrayRank();
            var dimensionLengths = new int[rank];
            return Array.CreateInstance(type.GetElementType(), dimensionLengths);
        }

        Maybe<object> IMaybeAutoValueProvider.GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetValue(type));
        }
    }
}