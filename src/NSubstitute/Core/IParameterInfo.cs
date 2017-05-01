using System;

namespace NSubstitute.Core
{
    public interface IParameterInfo
    {
        Type ParameterType { get; }
        bool IsParams { get; }
        bool IsOptional { get; }
        bool IsOut { get; }
    }
}