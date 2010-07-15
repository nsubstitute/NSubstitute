using System;
using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public interface ICallHandlerFactory
    {
        ICallHandler CreateCallHandler(Type handlerType, ISubstituteState substituteState, object[] routeArguments);
    }
}