using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public interface IAutoValueProvider
    {
        bool CanProvideValueFor(Type type);
        object GetValue(Type type);
    }

    public interface IMaybeAutoValueProvider
    {
        Maybe<object> GetValue(Type type);
    }
}