using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes
{
    public class RouteSpecs
    {
        public class When_handling_a_call : ConcernFor<Route>
        {
            private object _result;
            private ICall _call;
            private ICallHandler _firstHandler;
            private ICallHandler _secondHandler;
            private readonly object _lastHandlerResult = new object();
            private ICallHandler[] _handlers;

            [Test]
            public void Should_return_result_from_last_handler()
            {
                Assert.That(_result, Is.SameAs(_lastHandlerResult)); 
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _firstHandler = mock<ICallHandler>();
                _secondHandler = mock<ICallHandler>();
                _secondHandler.stub(x => x.Handle(_call)).Return(_lastHandlerResult);

                _handlers = new[] { _firstHandler, _secondHandler };
            }

            public override Route CreateSubjectUnderTest()
            {
                return new Route(_handlers);
            }
        }

        public class For_all_route_types : StaticConcern
        {
            private IEnumerable<Type> _allRouteTypes;

            [Test]
            public void Should_have_a_single_constructor_that_takes_route_parts_as_its_only_argument()
            {
                foreach (var routeType in _allRouteTypes)
                {
                    AssertHasOneConstructor(routeType);
                    AssertConstructorContainsSingleRoutePartsArgument(routeType);
                }
            }

            public override void Context()
            {
                var assemblyContainingRoutes = Assembly.GetAssembly(typeof(IRoute));
                _allRouteTypes = assemblyContainingRoutes.GetTypes().Where(x => ImplementsRouteInterface(x)).Where(x => x != typeof(Route));
            }

            private bool ImplementsRouteInterface(Type type)
            {
                if (type == typeof(IRoute)) return false;
                return typeof(IRoute).IsAssignableFrom(type);
            }

            private void AssertHasOneConstructor(Type type)
            {
                var numberOfPublicConstructors = type.GetConstructors().Length;
                Assert.That(numberOfPublicConstructors, Is.EqualTo(1), type.Name);
            }

            private void AssertConstructorContainsSingleRoutePartsArgument(Type type)
            {
                var constructor = type.GetConstructors().First();
                var constructorParameters = constructor.GetParameters();
                Assert.That(constructorParameters.Length, Is.EqualTo(1), type.Name);
                Assert.That(constructorParameters.First().ParameterType, Is.EqualTo(typeof(IRouteParts)), type.Name);
            }
        }
    }
}