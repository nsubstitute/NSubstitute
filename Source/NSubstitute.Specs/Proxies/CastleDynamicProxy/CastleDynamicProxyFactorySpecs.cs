using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using Rhino.Mocks;
using NSubstitute.Proxies.CastleDynamicProxy;
using NUnit.Framework;

namespace NSubstitute.Specs.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactorySpecs
    {
        public abstract class When_creating_a_proxy<TFoo> : ConcernFor<CastleDynamicProxyFactory> where TFoo : class, IFoo
        {
            ICallRouter _callRouter;
            TFoo _result;

            public override void Context()
            {
                _callRouter = mock<ICallRouter>();
            }

            public override void Because()
            {
                _result = sut.GenerateProxy<TFoo>(_callRouter);
            }

            [Test]
            public void Should_generate_a_proxy_that_forwards_to_call_router()
            {
                _result.Goo();
                _callRouter.received(x => x.Route(Arg<ICall>.Matches(call => CallWasToMethodNamed(call, "Goo"))));
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
                _callRouter
                    .stub(x => x.Route(Arg<ICall>.Matches(
                        call => CallWasToMethodNamed(call, "Bar") && CallArgsWere(call, new object[] {aNumber, aString})
                        )
                    ))
                    .Return(returnFromRoute);

                var returnValue = _result.Bar(aNumber, aString);
                Assert.That(returnValue, Is.EqualTo(returnFromRoute));                
            }

            public override CastleDynamicProxyFactory CreateSubjectUnderTest()
            {
                return new CastleDynamicProxyFactory(new CastleInterceptorFactory());
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
                resultsCallRouter.SetRoute<IRoute>();
                _callRouter.received(x => x.SetRoute<IRoute>());
            }
        }

        public class When_creating_a_proxy_for_an_interface : When_creating_a_proxy<IFoo>
        {
        }

        public class When_creating_a_proxy_for_a_class : When_creating_a_proxy<Foo>
        {
        }
    }
}