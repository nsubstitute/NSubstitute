using System;

namespace NSubstitute.Core
{
    // better naming
    public interface ISubstituteContext
    {
        Func<ICall, RouteAction>[] CustomCallHandlers { get; }
        void EnqueueCustomCallHandler(Func<ICall, RouteAction> customCallHandler);
    }
}