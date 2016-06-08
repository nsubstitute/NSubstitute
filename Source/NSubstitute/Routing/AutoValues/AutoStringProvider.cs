using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoStringProvider : IAutoValueProvider
    {
        private bool CanProvideValueFor(Type type)
        {
            return type == typeof(string); 
        }

        private object GetActualValue(Type type)
        {
            return string.Empty;
        }

        public Maybe<object> GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetActualValue(type));
        }
    }
}