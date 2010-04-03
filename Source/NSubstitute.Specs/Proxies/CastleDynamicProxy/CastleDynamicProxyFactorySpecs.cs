extern alias CastleCore;
using Castle.DynamicProxy;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using Rhino.Mocks;
using NSubstitute.Proxies.CastleDynamicProxy;
using NUnit.Framework;

namespace NSubstitute.Specs.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactorySpecs
    {
        public abstract class Concern : ConcernFor<CastleDynamicProxyFactory>
        {
            protected ICallHandler _callHandler;

            public override void Context()
            {
                _callHandler = mock<ICallHandler>();
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(new ProxyGenerator(), new CastleInterceptorFactory());
            }

            protected void AssertCallsMadeToResultsCallHandlerAreForwardedToOriginalHandler(object result)
            {
                var resultsCallHandler = (ICallHandler) result;
                resultsCallHandler.AssertNextCallHasBeenReceived();
                _callHandler.received(x => x.AssertNextCallHasBeenReceived());
            }
        }

        public class When_creating_a_proxy_for_an_interface : Concern
        {
            IFoo _result;

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_handler()
            {
                _result.Goo();
                _callHandler.AssertWasCalled(x => x.Handle(Arg<ICall>.Matches(arg => arg.GetMethodInfo().Name == "Goo")));
            }

            [Test]
            public void Should_be_able_to_cast_proxy_to_its_call_handler()
            {
                Assert.That(_result, Is.InstanceOf<ICallHandler>());
                AssertCallsMadeToResultsCallHandlerAreForwardedToOriginalHandler(_result);
            }

            public override void Because()
            {
                _result = sut.GenerateProxy<IFoo>(_callHandler);
            }
        }

        public class When_creating_a_proxy_for_a_class : Concern
        {
            Foo _result;

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_handler()
            {
                _result.Goo();
                _callHandler.AssertWasCalled(x => x.Handle(Arg<ICall>.Matches(arg => arg.GetMethodInfo().Name == "Goo")));
            }

            [Test]
            public void Should_be_able_to_cast_proxy_to_its_call_handler()
            {
                Assert.That(_result, Is.InstanceOf<ICallHandler>());
                AssertCallsMadeToResultsCallHandlerAreForwardedToOriginalHandler(_result);
            }

            public override void Because()
            {
                _result = sut.GenerateProxy<Foo>(_callHandler);
            }
        }
    }
}