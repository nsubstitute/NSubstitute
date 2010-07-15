using System;
using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public interface ICallHandlerFactory
    {
        ICallHandler CreateCallHandler(Type partType, ISubstituteState substituteState, object[] routeArguments);
    }
}