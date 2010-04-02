extern alias CastleCore;
using System.Collections.Generic;
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
            protected ICallHandler callHandler;

            public override void Context()
            {
                callHandler = mock<ICallHandler>();
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(new ProxyGenerator(), new CastleInterceptorFactory());
            }

            protected void AssertCallsMadeToResultsCallHandlerAreForwardedToOriginalHandler(object result)
            {
                var resultsCallHandler = (ICallHandler) result;
                resultsCallHandler.AssertNextCallHasBeenReceived();
                callHandler.received(x => x.AssertNextCallHasBeenReceived());
            }
        }

        public class When_creating_a_proxy_for_an_interface : Concern
        {
            IFoo result;

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_handler()
            {
                result.Goo();
                callHandler.AssertWasCalled(x => x.Handle(Arg<ICall>.Matches(arg => arg.GetMethodInfo().Name == "Goo")));
            }

            [Test]
            public void Should_be_able_to_cast_proxy_to_its_call_handler()
            {
                Assert.That(result, Is.InstanceOf<ICallHandler>());
                AssertCallsMadeToResultsCallHandlerAreForwardedToOriginalHandler(result);
            }

            public override void Because()
            {
                result = sut.GenerateProxy<IFoo>(callHandler);
            }
        }

        public class When_creating_a_proxy_for_a_class : Concern
        {
            Foo result;

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_handler()
            {
                result.Goo();
                callHandler.AssertWasCalled(x => x.Handle(Arg<ICall>.Matches(arg => arg.GetMethodInfo().Name == "Goo")));
            }

            [Test]
            public void Should_be_able_to_cast_proxy_to_its_call_handler()
            {
                Assert.That(result, Is.InstanceOf<ICallHandler>());
                AssertCallsMadeToResultsCallHandlerAreForwardedToOriginalHandler(result);
            }

            public override void Because()
            {
                result = sut.GenerateProxy<Foo>(callHandler);
            }
        }
    }
}