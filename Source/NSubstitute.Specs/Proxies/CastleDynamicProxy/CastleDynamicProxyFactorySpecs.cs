using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactorySpecs
    {
        public abstract class When_creating_a_proxy<TFoo> : ConcernFor<CastleDynamicProxyFactory> where TFoo : class, IFoo
        {
            ICallRouter _callRouter;
            protected TFoo _result;
            protected Type[] _additionalInterfaces;
            protected object[] _ctorArgs;

            public override void Context()
            {
                _callRouter = mock<ICallRouter>();
            }

            public override void Because()
            {
                _result = (TFoo) sut.GenerateProxy(_callRouter, typeof(TFoo), _additionalInterfaces, _ctorArgs);
            }

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_router()
            {
                _result.Goo();
                var call = Arg<ICall>.Matches(c => CallWasToMethodNamed(c, "Goo"));
                _callRouter.received(x => x.Route(call));
            }

            [Test]
            public void Should_be_able_to_cast_proxy_to_its_call_router()
            {
                Assert.That(_result, Is.InstanceOf<ICallRouter>());
                AssertCallsMadeToResultsCallRouterAreForwardedToOriginalRouter(_result);
            }

            [Test]
            public void Should_forward_call_with_original_arguments_to_router_and_return_value_from_route()
            {
                const int aNumber = 5;
                const string aString = "Some string";
                const string returnFromRoute = "value from route";

                var call = Arg<ICall>.Matches(c => CallWasToMethodNamed(c, "Bar") && CallArgsWere(c, new object[] {aNumber, aString}));

                _callRouter.stub(x => x.Route(call)).Return(returnFromRoute);

                var returnValue = _result.Bar(aNumber, aString);
                Assert.That(returnValue, Is.EqualTo(returnFromRoute));                
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory();
            }

            private bool CallWasToMethodNamed(ICall call, string methodName)
            {
                return call.GetMethodInfo().Name == methodName;
            }

            private bool CallArgsWere(ICall call, object[] expectedArguments)
            {
                var callArguments = call.GetArguments();
                if (callArguments.Length != expectedArguments.Length) return false;
                for (int i = 0; i < callArguments.Length; i++)
                {
                    if (!callArguments[i].Equals(expectedArguments[i])) return false;
                }
                return true;
            }

            protected void AssertCallsMadeToResultsCallRouterAreForwardedToOriginalRouter(object result)
            {
                var resultsCallRouter = (ICallRouter) result;
                var newCall = mock<ICall>();
                resultsCallRouter.Route(newCall);
                _callRouter.received(x => x.Route(newCall));
            }
        }

        public class When_creating_a_proxy_for_an_interface : When_creating_a_proxy<IFoo>
        {
        }

        public class When_creating_a_proxy_for_a_class : When_creating_a_proxy<Foo>
        {
        }

        public class When_providing_additional_interfaces : When_creating_a_proxy<IFoo>
        {
            public override void Context()
            {
                base.Context();
                _additionalInterfaces = new[] { typeof(IFirstExtra), typeof(ISecondExtra) };
            }

            [Test]
            public void Proxy_should_also_be_instance_of_each_additional_interface()
            {
                Assert.IsInstanceOf<IFirstExtra>(_result); 
                Assert.IsInstanceOf<ISecondExtra>(_result); 
            }
        }

        public class When_creating_a_substitute_and_passing_a_class_as_an_additional_interface : StaticConcern
        {
            [Test]
            public void Should_throw_exception_as_proxies_should_not_inherit_from_multiple_concrete_classes()
            {
                var sut = new CastleDynamicProxyFactory();

                Assert.Throws<SubstituteException>(
                    () => sut.GenerateProxy(mock<ICallRouter>(), typeof(Foo), new[] { typeof(IFoo), typeof(SomeOtherClass) }, null)
                );
            }

            public class SomeOtherClass { }
        }

        public class When_substituting_for_an_interface_and_passing_constructor_arguments : StaticConcern
        {
            [Test]
            public void Should_throw_exception()
            {
                var sut = new CastleDynamicProxyFactory();

                Assert.Throws<SubstituteException>(
                    () => sut.GenerateProxy(mock<ICallRouter>(), typeof(IFoo), null, new[] { new object() }));
            }

        }

        public class When_creating_a_proxy_for_a_class_with_constructor_args : When_creating_a_proxy<FooWithCtorArgs>
        {
            public override void Context()
            {
                base.Context();
                _ctorArgs = new object[] { "string", 4 };
            }
        }

        public interface IFirstExtra { }
        public interface ISecondExtra { }
        public class FooWithCtorArgs : Foo { public FooWithCtorArgs(string s, int number) { } }
    }
}