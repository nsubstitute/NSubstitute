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
            protected ICallRouter _callRouter;

            public override void Context()
            {
                _callRouter = mock<ICallRouter>();
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(new ProxyGenerator(), new CastleInterceptorFactory());
            }

            protected void AssertCallsMadeToResultsCallRouterAreForwardedToOriginalRouter(object result)
            {
                var resultsCallRouter = (ICallRouter) result;
                resultsCallRouter.SetRoute<CheckCallReceivedRoute>();
                _callRouter.received(x => x.SetRoute<CheckCallReceivedRoute>());
            }
        }

        public class When_creating_a_proxy_for_an_interface : Concern
        {
            IFoo _result;

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_router()
            {
                _result.Goo();
                _callRouter.AssertWasCalled(x => x.Route(Arg<ICall>.Matches(arg => arg.GetMethodInfo().Name == "Goo")));
            }

            [Test]
            public void Should_be_able_to_cast_proxy_to_its_call_router()
            {
                Assert.That(_result, Is.InstanceOf<ICallRouter>());
                AssertCallsMadeToResultsCallRouterAreForwardedToOriginalRouter(_result);
            }

            public override void Because()
            {
                _result = sut.GenerateProxy<IFoo>(_callRouter);
            }
        }

        public class When_creating_a_proxy_for_a_class : Concern
        {
            Foo _result;

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_router()
            {
                _result.Goo();
                _callRouter.AssertWasCalled(x => x.Route(Arg<ICall>.Matches(arg => arg.GetMethodInfo().Name == "Goo")));
            }

            [Test]
            public void Should_be_able_to_cast_proxy_to_its_call_router()
            {
                Assert.That(_result, Is.InstanceOf<ICallRouter>());
                AssertCallsMadeToResultsCallRouterAreForwardedToOriginalRouter(_result);
            }

            public override void Because()
            {
                _result = sut.GenerateProxy<Foo>(_callRouter);
            }
        }
    }
}