using System;

namespace NSubstitute.Core.Arguments
{
    public interface IDefaultChecker
    {
        bool IsDefault(object? value, Type forType);
    }
}