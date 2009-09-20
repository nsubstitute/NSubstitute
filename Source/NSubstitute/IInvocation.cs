using System;

namespace NSubstitute
{
    public interface IInvocation
    {
        Type GetReturnType();
    }
}