using System;
using System.Reflection;

namespace NSubstitute.Core
{
    public interface ICall
    {
        Type GetReturnType();
        MethodInfo GetMethodInfo();
        object[] GetArguments();
        object Target();
        Type[] GetParameterTypes();
    }
}