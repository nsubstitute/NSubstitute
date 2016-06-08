#if NET45
using System;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoObservableProvider : IAutoValueProvider, IMaybeAutoValueProvider
    {
        private readonly Func<IAutoValueProvider[]> _autoValueProviders;
        private readonly Func<IMaybeAutoValueProvider[]> _maybeAutoValueProviders;

        public AutoObservableProvider(Func<IAutoValueProvider[]> autoValueProviders)
        {
            _autoValueProviders = autoValueProviders;
        }

        public AutoObservableProvider(Func<IAutoValueProvider[]> autoValueProviders, Func<IMaybeAutoValueProvider[]> maybeAutoValueProviders)
            : this(autoValueProviders)
        {
            _maybeAutoValueProviders = maybeAutoValueProviders;
        }

        public bool CanProvideValueFor(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IObservable<>);
        }

        public object GetValue(Type type)
        {
            if (!CanProvideValueFor(type)) 
                throw new InvalidOperationException();

            Type innerType = type.GetGenericArguments()[0];
            var valueProvider = _autoValueProviders().FirstOrDefault(vp => vp.CanProvideValueFor(innerType));
            var value = valueProvider == null ? GetDefault(innerType) : valueProvider.GetValue(innerType);
            return Activator.CreateInstance(
                    typeof(ReturnObservable<>).MakeGenericType(innerType)
                    , new object[] { value });
        }

        private object GetDefault(Type type)
        {
            if (_maybeAutoValueProviders == null)
                return null;

            return _maybeAutoValueProviders()
                .Select(vp => vp.GetValue(type))
                .FirstOrDefault(vp => vp.HasValue())
                .ValueOrDefault();
        }

        Maybe<object> IMaybeAutoValueProvider.GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetValue(type));
        }
    }
}
#endif