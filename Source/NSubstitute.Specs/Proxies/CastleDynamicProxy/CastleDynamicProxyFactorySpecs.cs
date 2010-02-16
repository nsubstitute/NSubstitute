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
            protected IInvocationHandler invocationHandler;
            protected CastleInterceptorFactory interceptorFactory;

            public override void Context()
            {
                invocationHandler = mock<IInvocationHandler>();
                interceptor = mock<CastleIInterceptor>();
                interceptorFactory = mock<CastleInterceptorFactory>();
                interceptorFactory.stub(x => x.CreateForwardingInterceptor(invocationHandler)).Return(interceptor);
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(new ProxyGenerator(), interceptorFactory);
            }

            protected void AssertCallsMadeToResultsInvocationHandlerAreForwardedToOriginalHandler(object result)
            {
                var resultsInvocationHandler = (IInvocationHandler) result;
                resultsInvocationHandler.AssertNextCallHasBeenReceived();
                invocationHandler.received(x => x.AssertNextCallHasBeenReceived());
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
            public void Should_be_able_to_cast_proxy_to_its_invocation_handler()
            {
                Assert.That(result, Is.InstanceOf<IInvocationHandler>());
                AssertCallsMadeToResultsInvocationHandlerAreForwardedToOriginalHandler(result);
            }

            public override void Because()
            {
                result = sut.GenerateProxy<IFoo>(invocationHandler);
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
            public void Should_be_able_to_cast_proxy_to_its_invocation_handler()
            {
                Assert.That(result, Is.InstanceOf<IInvocationHandler>());
                AssertCallsMadeToResultsInvocationHandlerAreForwardedToOriginalHandler(result);
            }

            public override void Because()
            {
                result = sut.GenerateProxy<Foo>(invocationHandler);
            }
        }
    }
}