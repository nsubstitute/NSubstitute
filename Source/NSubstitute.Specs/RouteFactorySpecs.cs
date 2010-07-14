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
            private IRouteParts _routeParts;

            [Test]
            public void Should_create_route_with_route_parts()
            {
                Assert.That(_result, Is.SameAs(SampleRoute.LastInstanceCreated));
                Assert.That(((SampleRoute) _result).RoutePartsUsedForCreation, Is.SameAs(_routeParts));
            }

            public override void Because()
            {
                _result = sut.Create<SampleRoute>(_routeArguments);
            }

            public override void Context()
            {
                _routeArguments = new[] {new object(), new object()};
                _routeParts = mock<IRouteParts>();
                _routePartsFactory = mock<IRoutePartsFactory>();
                _routePartsFactory.stub(x => x.Create(_routeArguments)).Return(_routeParts);
            }

            public override RouteFactory CreateSubjectUnderTest()
            {
                return new RouteFactory(_routePartsFactory);
            }

            class SampleRoute : IRoute, IRouteDefinition {
                public static SampleRoute LastInstanceCreated { get; private set; }
                public IRouteParts RoutePartsUsedForCreation { get; private set; }
                public SampleRoute(IRouteParts routeParts)
                {
                    LastInstanceCreated = this;
                    RoutePartsUsedForCreation = routeParts;
                }
                public object Handle(ICall call) { return null; }

                public IEnumerable<Type> HandlerTypes { get { return new Type[0]; } }
            }
        }
    }
}