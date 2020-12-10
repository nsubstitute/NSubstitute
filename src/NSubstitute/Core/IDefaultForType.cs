using System;

namespace NSubstitute.Core
{
    public interface IDefaultForType
    {
        object? GetDefaultFor(Type type);
    }
}