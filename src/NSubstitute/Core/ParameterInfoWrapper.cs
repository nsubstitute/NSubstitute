using System.Reflection;

namespace NSubstitute.Core;

internal sealed class ParameterInfoWrapper(ParameterInfo parameterInfo) : IParameterInfo
{
    public Type ParameterType => parameterInfo.ParameterType;

    public bool IsParams => parameterInfo.IsParams();

    public bool IsOptional => parameterInfo.IsOptional;

    public bool IsOut => parameterInfo.IsOut;
}