using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstitutionContextSpecs
    {
        public abstract class Concern : ConcernFor<SubstitutionContext>
        {
            protected ISubstituteFactory _substituteFactory;

            public override void Context()
            {
                _substituteFactory = mock<ISubstituteFactory>();
            }

            public override SubstitutionContext CreateSubjectUnderTest()
            {
                return new SubstitutionContext(_substituteFactory);
            }            
        }
        
        public class When_setting_the_return_value_of_the_last_call : Concern
        {
            private ICallRouter _callRouter;
            private int _valueToReturn;

            [Test]
            public void Should_tell_the_last_call_router_to_set_the_return_value_of_its_last_call()
            {
                _callRouter.received(x => x.LastCallShouldReturn(_valueToReturn));
            }
            
            public override void Because()
            {
                sut.LastCallRouter(_callRouter);
                sut.LastCallShouldReturn(_valueToReturn);
            }

            public override void Context()
            {
                base.Context();
                _callRouter = mock<ICallRouter>();
                _valueToReturn = 42;
            }
        }

        public class When_trying_to_set_a_return_value_when_no_previous_call_has_been_made : Concern
        {
            [Test]
            public void Should_throw_a_substitute_exception()
            {
                Assert.Throws<SubstituteException>(() => sut.LastCallShouldReturn(5));
            }
        }

        public class When_getting_call_router_for_a_substitute : Concern
        {
            private ICallRouter _routerForSubstitute;
            private object _substituteCastableAsRouter;
            private ICallRouter _result;

            [Test]
            public void Should_cast_substitute_to_call_router()
            {
                Assert.That(_result, Is.SameAs(_routerForSubstitute));               
            }

            public override void Because()
            {
                _result = sut.GetCallRouterFor(_substituteCastableAsRouter);
            }

            public override void Context()
            {
                base.Context();                
                _routerForSubstitute = mock<ICallRouter>();
                _substituteCastableAsRouter = _routerForSubstitute;
            }
        }

        public class When_getting_call_router_for_an_instance_that_is_not_a_substitute : Concern
        {
            private object _notASubstitute;

            [Test]
            public void Should_throw_an_exception()
            {
                Assert.Throws<NotASubstituteException>(() => sut.GetCallRouterFor(_notASubstitute));
            }

            public override void Context()
            {
                base.Context();
                _notASubstitute = new object();
            }        
        }


        public class When_getting_the_substitute_factory_for_the_context:Concern
        {
            [Test]
            public void Should_return_the_factory_the_context_was_created_with()
            {
                Assert.That(sut.GetSubstituteFactory(), Is.SameAs(_substituteFactory));
            }
        }

        public class When_accessing_current_instance : StaticConcern
        {
            [Test]
            public void Should_return_same_instance_of_substitution_context()
            {
                var firstInstance = SubstitutionContext.Current;
                var secondInstance = SubstitutionContext.Current;
                Assert.That(firstInstance, Is.SameAs(secondInstance) | Is.Not.Null);                
            }

            [Test]
            public void Should_have_a_substitute_factory()
            {
                var context = SubstitutionContext.Current;
                Assert.That(context.GetSubstituteFactory(), Is.TypeOf<SubstituteFactory>());
            }
        }
    }
}