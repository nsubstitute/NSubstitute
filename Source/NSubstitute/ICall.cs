using System;
using System.Reflection;

namespace NSubstitute
{
    public interface ICall
    {
        Type GetReturnType();
        MethodInfo GetMethodInfo();
        object[] GetArguments();
        object Target();
    }
}