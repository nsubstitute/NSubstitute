using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes
{
    public class RouteSpecs
    {
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
                _allRouteTypes = assemblyContainingRoutes.GetTypes().Where(x => ImplementsRouteInterface(x));
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