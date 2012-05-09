using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public interface ICallRouter
    {
        void LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs);
        object Route(ICall call);
        void SetRoute<TRouteDefinition>(params object[] routeArguments) where TRouteDefinition : IRouteDefinition;
        void ClearReceivedCalls();
        IEnumerable<ICall> ReceivedCalls();
        IEnumerable<ICall> ReceivedCalls<T>(Expression<Action<T>> call);
    }
}