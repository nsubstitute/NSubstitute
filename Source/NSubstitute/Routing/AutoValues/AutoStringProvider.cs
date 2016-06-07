using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoStringProvider : IAutoValueProvider, IMaybeAutoValueProvider
    {
        public bool CanProvideValueFor(Type type)
        {
            return type == typeof(string); 
        }

        public object GetValue(Type type)
        {
            return string.Empty;
        }

        Maybe<object> IMaybeAutoValueProvider.GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetValue(type));
        }
    }
}