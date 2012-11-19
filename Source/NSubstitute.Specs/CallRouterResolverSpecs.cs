using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallRouterResolverSpecs
    {
        public class Concern : ConcernFor<CallRouterResolver>
        {
            protected ICallRouter _routerForSubstitute;

            public override CallRouterResolver CreateSubjectUnderTest()
            {
                return new CallRouterResolver();
            }

            public override void Context()
            {
                base.Context();
                _routerForSubstitute = mock<ICallRouter>();
            }
        }

        public class When_getting_call_router_for_a_substitute_that_implements_call_router : Concern
        {
            private object _substituteCastableAsRouter;
            private ICallRouter _result;

            [Test]
            public void Should_cast_substitute_to_call_router()
            {
                Assert.That(_result, Is.SameAs(_routerForSubstitute));
            }

            public override void Because()
            {
                _result = sut.ResolveFor(_substituteCastableAsRouter);
            }

            public override void Context()
            {
                base.Context();
                _substituteCastableAsRouter = _routerForSubstitute;
            }
        }

        public class When_getting_call_router_for_a_substitute_that_implements_call_router_provider : Concern
        {
            private ICallRouterProvider _substituteThatCanProviderARouter;
            private ICallRouter _result;

            [Test]
            public void Should_cast_substitute_to_call_router()
            {
                Assert.That(_result, Is.SameAs(_routerForSubstitute));
            }

            public override void Because()
            {
                _result = sut.ResolveFor(_substituteThatCanProviderARouter);
            }

            public override void Context()
            {
                base.Context();
                _substituteThatCanProviderARouter = mock<ICallRouterProvider>();
                _substituteThatCanProviderARouter.stub(x => x.CallRouter).Return(_routerForSubstitute);
            }
        }

        public class When_getting_call_router_for_substitute_that_does_not_implement_call_router_but_has_been_registered : Concern
        {
            private object _substitute;
            private ICallRouter _result;

            [Test]
            public void Should_return_registered_call_router()
            {
                Assert.That(_result, Is.SameAs(_routerForSubstitute));
            }

            public override void Because()
            {
                sut.Register(_substitute, _routerForSubstitute);
                _result = sut.ResolveFor(_substitute);
            }

            public override void Context()
            {
                base.Context();
                _substitute = new object();
            }
        }

        public class When_getting_call_router_for_an_object_that_is_not_a_substitute : Concern
        {
            [Test]
            public void Should_throw_a_not_a_substitute_exception()
            {
                Assert.Throws<NotASubstituteException>(() =>
                    sut.ResolveFor(new object())
                );
            }
        }

        public class When_getting_call_router_for_a_null_reference : Concern
        {
            [Test]
            public void Should_throw_a_null_substitute_reference_exception()
            {
                Assert.Throws<NullSubstituteReferenceException>(() => 
                    sut.ResolveFor(null)
                );
            }
        }
    }
}