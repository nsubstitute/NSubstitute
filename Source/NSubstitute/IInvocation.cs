using System;
using System.Reflection;

namespace NSubstitute
{
    public interface IInvocation
    {
        Type GetReturnType();
        MethodInfo MethodInfo { get; }
    }
}