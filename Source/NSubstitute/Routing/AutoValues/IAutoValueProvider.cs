using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public interface IAutoValueProvider
    {
        Maybe<object> GetValue(Type type);
    }
}