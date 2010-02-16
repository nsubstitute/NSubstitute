using System;
using NSubstitute.Exceptions;
using NSubstitute.Specs.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstitutionContextSpecs
    {
        public abstract class Concern : ConcernFor<SubstitutionContext>
        {
            protected ISubstituteFactory substituteFactory;

            public override void Context()
            {
                substituteFactory = mock<ISubstituteFactory>();
            }

            public override SubstitutionContext CreateSubjectUnderTest()
            {
                return new SubstitutionContext(substituteFactory);
            }            
        }
        
        public class When_setting_the_return_value_of_the_last_invocation : Concern
        {
            private IInvocationHandler invocationHandler;
            private int valueToReturn;

            [Test]
            public void Should_tell_the_last_invocation_handler_to_set_the_return_value_of_its_last_invocation()
            {
                invocationHandler.received(x => x.LastInvocationShouldReturn(valueToReturn));
            }
            
            public override void Because()
            {
                sut.LastInvocationHandlerInvoked(invocationHandler);
                sut.LastInvocationShouldReturn(valueToReturn);
            }

            public override void Context()
            {
                base.Context();
                invocationHandler = mock<IInvocationHandler>();
                valueToReturn = 42;
            }
        }

        public class When_trying_to_set_a_return_value_when_no_previous_invocation_has_been_made : Concern
        {
            [Test]
            public void Should_throw_a_substitute_exception()
            {
                Assert.Throws<SubstituteException>(() => sut.LastInvocationShouldReturn(5));
            }
        }

        public class When_getting_invocation_handler_for_a_substitute : Concern
        {
            private IInvocationHandler _handlerForSubstitute;
            private object _substituteCastableAsHandler;
            private IInvocationHandler result;

            [Test]
            public void Should_cast_substitute_to_invocation_handler()
            {
                Assert.That(result, Is.SameAs(_handlerForSubstitute));               
            }

            public override void Because()
            {
                result = sut.GetInvocationHandlerFor(_substituteCastableAsHandler);
            }

            public override void Context()
            {
                base.Context();                
                _handlerForSubstitute = mock<IInvocationHandler>();
                _substituteCastableAsHandler = _handlerForSubstitute;
            }
        }

        public class When_getting_invocation_handler_for_an_instance_that_is_not_a_substitute : Concern
        {
            private object _notASubstitute;

            [Test]
            public void Should_throw_an_exception()
            {
                Assert.Throws<NotASubstituteException>(() => sut.GetInvocationHandlerFor(_notASubstitute));
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
                Assert.That(sut.GetSubstituteFactory(), Is.SameAs(substituteFactory));
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