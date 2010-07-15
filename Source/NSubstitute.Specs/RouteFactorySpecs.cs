using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class RouteFactorySpecs
    {
        public class When_creating_a_route : ConcernFor<RouteFactory>
        {
            private object[] _routeArguments;
            private IRoute _result;
            private IRoutePartsFactory _routePartsFactory;
            private ICallHandlerFactory _callHandlerFactory;
            private ICallHandler[] _callHandlers;
            private ISubstituteState _substituteState;

            [Test]
            public void Should_create_route_with_from_defined_handlers()
            {
                var route = (FakeRoute) _result;
                Assert.That(route.CallHandlers, Is.EqualTo(_callHandlers));
            }

            public override void Because()
            {
                _result = sut.Create<SampleRouteDefinition>(_routeArguments);
            }

            public override void Context()
            {
                _substituteState = mock<ISubstituteState>();
                _callHandlers = new[] { mock<ICallHandler>(), mock<ICallHandler>() };
                _routeArguments = new[] {new object(), new object()};

                _callHandlerFactory = mock<ICallHandlerFactory>();
                _routePartsFactory = mock<IRoutePartsFactory>();
                _routePartsFactory.stub(x => x.Create(_routeArguments)).Return(_callHandlerFactory);
                _callHandlerFactory.stub(x => x.CreateCallHandler(SampleRouteDefinition.Handlers[0], _substituteState, _routeArguments)).Return(_callHandlers[0]);
                _callHandlerFactory.stub(x => x.CreateCallHandler(SampleRouteDefinition.Handlers[1], _substituteState, _routeArguments)).Return(_callHandlers[1]);

                temporarilyChange(() => RouteFactory.ConstructRoute).to(x => new FakeRoute(x));
            }

            public override RouteFactory CreateSubjectUnderTest()
            {
                return new RouteFactory(_substituteState, _routePartsFactory);
            }

            class FakeRoute : IRoute
            {
                public IEnumerable<ICallHandler> CallHandlers { get; private set; }
                public FakeRoute(IEnumerable<ICallHandler> callHandlers) { CallHandlers = callHandlers; }
                public object Handle(ICall call) { throw new NotImplementedException(); }
            }

            class SampleRouteDefinition : IRouteDefinition {
                public static Type[] Handlers = new[] { typeof(object), typeof(int) };
                public IEnumerable<Type> HandlerTypes { get { return Handlers; } }
            }
        }
    }
}