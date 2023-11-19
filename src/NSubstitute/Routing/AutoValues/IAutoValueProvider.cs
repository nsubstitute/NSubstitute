using System;

namespace NSubstitute.Routing.AutoValues
{
    public interface IAutoValueProvider
    {
        bool CanProvideValueFor(Type type);
        object? GetValue(Type type);
    }
}