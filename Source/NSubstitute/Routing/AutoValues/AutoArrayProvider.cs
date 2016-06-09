using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoArrayProvider : IAutoValueProvider
    {
        private bool CanProvideValueFor(Type type)
        {
            return type.IsArray;
        }

        private object GetActualValue(Type type)
        {
            var rank = type.GetArrayRank();
            var dimensionLengths = new int[rank];
            return Array.CreateInstance(type.GetElementType(), dimensionLengths);
        }

        public Maybe<object> GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetActualValue(type));
        }
    }
}