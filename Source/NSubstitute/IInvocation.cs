using System;
using System.Collections;
using System.Reflection;

namespace NSubstitute
{
    public interface IInvocation
    {
        Type GetReturnType();
        MethodInfo GetMethodInfo();
        object[] GetArguments();
    }
}