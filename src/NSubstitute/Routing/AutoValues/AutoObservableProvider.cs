using NSubstitute.Core;
using System.Reflection;

namespace NSubstitute.Routing.AutoValues;

public class AutoObservableProvider : IAutoValueProvider
{
    private readonly Lazy<IReadOnlyCollection<IAutoValueProvider>> _autoValueProviders;

    public AutoObservableProvider(Lazy<IReadOnlyCollection<IAutoValueProvider>> autoValueProviders)
    {
        _autoValueProviders = autoValueProviders;
    }

    public bool CanProvideValueFor(Type type) =>
        type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IObservable<>);

    public object? GetValue(Type type)
    {
        if (!CanProvideValueFor(type))
            throw new InvalidOperationException();

        Type innerType = type.GetGenericArguments()[0];
        var valueProvider = _autoValueProviders.Value.FirstOrDefault(vp => vp.CanProvideValueFor(innerType));
        var value = valueProvider == null ? GetDefault(type) : valueProvider.GetValue(innerType);
        return Activator.CreateInstance(typeof(ReturnObservable<>).MakeGenericType(innerType), [value]);
    }

    private static object? GetDefault(Type type)
    {
        return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }
}
