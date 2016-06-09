#if NET45
using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoObservableProvider : IAutoValueProvider
    {
        private readonly Func<IAutoValueProvider> _autoValueProviders;

        public AutoObservableProvider(Func<IAutoValueProvider> autoValueProviders)
        {
            _autoValueProviders = autoValueProviders;
        }

        private bool CanProvideValueFor(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IObservable<>);
        }

        private object GetActualValue(Type type)
        {
            Type innerType = type.GetGenericArguments()[0];
            var value = GetValueFromProvider(innerType);
            return Activator.CreateInstance(
                    typeof(ReturnObservable<>).MakeGenericType(innerType)
                    , new object[] { value });
        }

        private object GetValueFromProvider(Type type)
        {
            return _autoValueProviders()
                .GetValue(type)
                .ValueOrDefault();
        }

        public Maybe<object> GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetActualValue(type));
        }
    }
}
#endif