extern alias CastleCore;
using Castle.DynamicProxy;
using NSubstitute.Specs.TestInfrastructure;
using NSubstitute.Specs.TestStructures;
using Rhino.Mocks;
using CastleIInterceptor = CastleCore::Castle.Core.Interceptor.IInterceptor;
using CastleIInvocation = CastleCore::Castle.Core.Interceptor.IInvocation;
using NSubstitute.Proxies.CastleDynamicProxy;
using NUnit.Framework;

namespace NSubstitute.Specs.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactorySpecs
    {
        public abstract class Concern : ConcernFor<CastleDynamicProxyFactory>
        {
            protected CastleIInterceptor interceptor;
            protected ICallHandler callHandler;
            protected CastleInterceptorFactory interceptorFactory;

            public override void Context()
            {
                callHandler = mock<ICallHandler>();
                interceptor = mock<CastleIInterceptor>();
                interceptorFactory = mock<CastleInterceptorFactory>();
                interceptorFactory.stub(x => x.CreateForwardingInterceptor(callHandler)).Return(interceptor);
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(new ProxyGenerator(), interceptorFactory);
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
            public void Should_generate_a_proxy_that_forwards_to_interceptor()
            {
                result.Goo();
                interceptor.AssertWasCalled(x => x.Intercept(Arg<CastleIInvocation>.Is.Anything));
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
            public void Should_generate_a_proxy_that_forwards_to_interceptor()
            {
                result.Goo();
                interceptor.AssertWasCalled(x => x.Intercept(Arg<CastleIInvocation>.Is.Anything));
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