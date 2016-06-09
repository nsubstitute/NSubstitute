using System;
using System.Collections.Generic;
using NSubstitute.Routing;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Core
{
    public interface ICallRouter
    {
        ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs);
        object Route(ICall call);
        void ClearReceivedCalls();
        IEnumerable<ICall> ReceivedCalls();
        void SetRoute(Func<ISubstituteState, IRoute> getRoute);
        void SetReturnForType(Type type, IReturn returnValue);
        IList<IAutoValueProvider> AutoValueProviders { get; }
        AutoValueBehaviour AutoValueBehaviour { get; set; }
    }
}