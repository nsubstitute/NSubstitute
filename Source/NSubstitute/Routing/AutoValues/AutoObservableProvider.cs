#if NET45 || NETSTANDARD1_5
using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoObservableProvider : IAutoValueProvider
    {
        private readonly Func<IAutoValueProvider[]> _autoValueProviders;

        public AutoObservableProvider(Func<IAutoValueProvider[]> autoValueProviders)
        {
            _autoValueProviders = autoValueProviders;
        }

        public bool CanProvideValueFor(Type type)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(IObservable<>);
        }

        public object GetValue(Type type)
        {
            if (!CanProvideValueFor(type)) 
                throw new InvalidOperationException();

            Type innerType = type.GetGenericArguments()[0];
            var valueProvider = _autoValueProviders().FirstOrDefault(vp => vp.CanProvideValueFor(innerType));
            var value = valueProvider == null ? GetDefault(type) : valueProvider.GetValue(innerType);
            return Activator.CreateInstance(
                    typeof(ReturnObservable<>).MakeGenericType(innerType)
                    , new object[] { value });
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType() ? Activator.CreateInstance(type) : null;
        }
    }
}
#endif