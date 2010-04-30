using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
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
                var assemblyContainingRoutes = Assembly.GetAssembly(typeof (IRoute));
                _allRouteTypes = assemblyContainingRoutes.GetTypes().Where(x => ImplementsRouteInterface(x));
            }

            private bool ImplementsRouteInterface(Type type)
            {
                if (type == typeof(IRoute)) return false;
                return typeof (IRoute).IsAssignableFrom(type);
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
                Assert.That(constructorParameters.First().ParameterType, Is.EqualTo(typeof (IRouteParts)), type.Name);
            }
        }

        public abstract class WithRoute<TRoute> : ConcernFor<IRoute> where TRoute : IRoute
        {
            TestRouteParts _routeParts;
            readonly object _expectedResult = new object();
            object _result;
            ICall _call;

            [Test]
            public void Should_return_expected_result()
            {
                Assert.That(_result, Is.SameAs(_expectedResult));
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _routeParts = new TestRouteParts();
            }

            public override IRoute CreateSubjectUnderTest()
            {
                return (IRoute) Activator.CreateInstance(typeof (TRoute), _routeParts);
            }

            protected void AssertPartHandledCall<TPart>() where TPart : ICallHandler
            {
                var part = _routeParts.GetPart<TPart>();
                part.received(x => x.Handle(_call));
            }

            protected void ExpectReturnValueToComeFromPart<TPart>() where TPart : ICallHandler
            {
                var part = _routeParts.GetPart<TPart>();
                part.stub(x => x.Handle(_call)).Return(_expectedResult);
            }

            class TestRouteParts : IRouteParts
            {
                readonly IDictionary<Type, ICallHandler> _parts = new Dictionary<Type, ICallHandler>();
                public ICallHandler GetPart<TPart>() where TPart : ICallHandler
                {
                    var partKey = typeof(TPart);
                    return _parts.ContainsKey(partKey) ? _parts[partKey] : MockPartAndReturn(partKey);
                }
                private ICallHandler MockPartAndReturn(Type partKey)
                {
                    var part = MockingAdaptor.Create<ICallHandler>();
                    _parts[partKey] = part;
                    return part;
                }
            }
        }

        public class When_recording_call : WithRoute<RecordReplayRoute>
        {
            [Test]
            public void Should_set_properties()
            {
                AssertPartHandledCall<PropertySetterHandler>();
            }

            [Test]
            public void Should_handle_event_subscriptions()
            {
                AssertPartHandledCall<EventSubscriptionHandler>();
            }

            [Test]
            public void Should_record_call()
            {
                AssertPartHandledCall<RecordCallHandler>();
            }

            public override void Context()
            {
                base.Context();
                ExpectReturnValueToComeFromPart<RecordCallHandler>();
            }
        }

        public class When_raising_event : WithRoute<RaiseEventRoute>
        {
            [Test]
            public void Should_raise_event()
            {
                AssertPartHandledCall<RaiseEventHandler>();
            }

            public override void Context()
            {
                base.Context();
                ExpectReturnValueToComeFromPart<ReturnDefaultForCallHandler>();
            }
        }

        public class When_checking_call_received : WithRoute<CheckCallReceivedRoute>
        {
            [Test]
            public void Should_check_call_received()
            {
                AssertPartHandledCall<CheckReceivedCallHandler>();
            }

            public override void Context()
            {
                base.Context();
                ExpectReturnValueToComeFromPart<ReturnDefaultForCallHandler>();
            }
        }

    }
}