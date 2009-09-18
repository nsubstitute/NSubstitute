using System;

namespace NSubstitute
{
    public interface IInvocation
    {
        void SetReturn(object valueToReturn);
        Type GetReturnType();
    }
}