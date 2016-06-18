using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing;

namespace NSubstitute.Specs.Infrastructure
{
    public class TestCallRouter : ICallRouter
    {
        public ConfiguredCall LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs)
        {
            return new ConfiguredCall(x => { });
        }
        public void Clear(ClearOptions clear) { }

        public IEnumerable<ICall> ReceivedCalls() { return new ICall[0]; }

        public readonly object RouteResult = new object();
        public ICall RoutedCall { get; private set; }
        public Func<ISubstituteState, IRoute> FactoryMethodUsedToSetRoute { get; private set; }
        public Type TypeUseForSetReturnForType { get; private set; }
        public IReturn ReturnValueUsedForSetReturnForType { get; private set; }

        public object Route(ICall call)
        {
            if (FactoryMethodUsedToSetRoute == null) throw new InvalidOperationException("no route set");
            RoutedCall = call;
            return RouteResult;
        }

        public void SetRoute(Func<ISubstituteState, IRoute> getRoute)
        {
            FactoryMethodUsedToSetRoute = getRoute;
        }

        public void SetReturnForType(Type type, IReturn returnValue)
        {
            TypeUseForSetReturnForType = type;
            ReturnValueUsedForSetReturnForType = returnValue;
        }
    }
}