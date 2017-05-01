using System.Threading;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
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
            MatchArgs _argMatching = MatchArgs.AsSpecifiedInCall;
            private ICallRouter _callRouter;
            private IReturn _valueToReturn;

            [Test]
            public void Should_tell_the_last_call_router_to_set_the_return_value_of_its_last_call()
            {
                _callRouter.received(x => x.LastCallShouldReturn(_valueToReturn, _argMatching));
            }

            [Test]
            public void Should_throw_if_trying_to_set_another_return_value_before_another_call_is_made_on_a_substitute()
            {
                Assert.Throws<CouldNotSetReturnDueToNoLastCallException>(() => sut.LastCallShouldReturn(mock<IReturn>(), MatchArgs.AsSpecifiedInCall));
            }

            public override void Because()
            {
                sut.LastCallRouter(_callRouter);
                sut.LastCallShouldReturn(_valueToReturn, _argMatching);
            }

            public override void Context()
            {
                base.Context();
                _callRouter = mock<ICallRouter>();
                _callRouter.stub(c => c.IsLastCallInfoPresent()).Return(true);

                _valueToReturn = mock<IReturn>();
            }
        }

        public class When_trying_to_set_a_return_value_when_no_previous_call_has_been_made : Concern
        {
            [Test]
            public void Should_throw()
            {
                Assert.Throws<CouldNotSetReturnDueToNoLastCallException>(() => sut.LastCallShouldReturn(mock<IReturn>(), MatchArgs.AsSpecifiedInCall));
            }
        }

        public class When_trying_to_set_a_return_value_when_router_cannot_configure_last_call : Concern
        {
            private ICallRouter _callRouter;

            [Test]
            public void Should_throw_that_last_call_cannot_be_configured()
            {
                Assert.Throws<CouldNotSetReturnDueToMissingInfoAboutLastCallException>(() => sut.LastCallShouldReturn(mock<IReturn>(), MatchArgs.AsSpecifiedInCall));
            }

            public override void Because()
            {
                base.Because();

                _callRouter.stub(c => c.IsLastCallInfoPresent()).Return(false);
                sut.LastCallRouter(_callRouter);
            }

            public override void Context()
            {
                base.Context();

                _callRouter = mock<ICallRouter>();
            }
        }

        public class When_getting_call_router_for_a_substitute : Concern
        {
            private ICallRouter _routerForSubstitute;
            private object _substitute;
            private ICallRouter _result;

            [Test]
            public void Should_resolve_call_router_for_substitute()
            {
                Assert.That(_result, Is.SameAs(_routerForSubstitute));
            }

            public override void Because()
            {
                _result = sut.GetCallRouterFor(_substitute);
            }

            public override void Context()
            {
                base.Context();
                _substitute = new object();
                _routerForSubstitute = mock<ICallRouter>();
                _substituteFactory.stub(x => x.GetCallRouterCreatedFor(_substitute)).Return(_routerForSubstitute);
            }
        }

        public class When_getting_the_substitute_factory_for_the_context : Concern
        {
            [Test]
            public void Should_return_the_factory_the_context_was_created_with()
            {
                Assert.That(sut.SubstituteFactory, Is.SameAs(_substituteFactory));
            }
        }

        public class When_arg_specs_enqueued : Concern
        {
            IArgumentSpecification _first;
            IArgumentSpecification _second;

            [Test]
            public void Dequeuing_should_return_all_enqueued_arg_specs()
            {
                Assert.That(sut.DequeueAllArgumentSpecifications(), Is.EquivalentTo(new[] { _first, _second }));
            }

            [Test]
            public void Dequeuing_should_mean_context_no_longer_has_arg_specs()
            {
                sut.DequeueAllArgumentSpecifications();
                Assert.That(sut.DequeueAllArgumentSpecifications(), Is.Empty);
            }

            public override void Because()
            {
                base.Because();
                sut.EnqueueArgumentSpecification(_first);
                sut.EnqueueArgumentSpecification(_second);
            }

            public override void Context()
            {
                base.Context();
                _first = mock<IArgumentSpecification>();
                _second = mock<IArgumentSpecification>();
            }
        }

        public class When_working_with_thread_static_pending_spec_info : Concern
        {
            PendingSpecificationInfo _specInfo;

            [Test]
            public void Storage_is_bound_to_thread()
            {
                var thread = new Thread(() => { sut.PendingSpecificationInfo = _specInfo; })
                {
                    IsBackground = true
                };

                thread.Start();
                thread.Join();

                Assert.That(sut.PendingSpecificationInfo, Is.Null);
            }

            [Test]
            public void Set_value_is_read()
            {
                sut.PendingSpecificationInfo = _specInfo;
                Assert.That(sut.PendingSpecificationInfo, Is.EqualTo(_specInfo));
            }

            public override void Context()
            {
                base.Context();

                _specInfo = PendingSpecificationInfo.FromLastCall(mock<ICall>());
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
                Assert.That(context.SubstituteFactory, Is.TypeOf<SubstituteFactory>());
            }
        }
    }
}