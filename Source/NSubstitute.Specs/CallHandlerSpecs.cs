using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallHandlerSpecs
    {
        public abstract class Concern : ConcernFor<CallHandler>
        {
            protected int valueToReturn;
            protected ISubstitutionContext context;
            protected ICall call;
            protected ICallStack CallStack;
            protected ICallResults configuredResults;
            protected IPropertyHelper PropertyHelper;
            protected ICallSpecification callSpecification;
            ICallSpecificationFactory callSpecificationFactory;

            public override void Context()
            {
                valueToReturn = 7;
                context = mock<ISubstitutionContext>();
                CallStack = mock<ICallStack>();
                configuredResults = mock<ICallResults>();
                PropertyHelper = mock<IPropertyHelper>();
                call = mock<ICall>();
                callSpecification = mock<ICallSpecification>();
                callSpecificationFactory = mock<ICallSpecificationFactory>();
                callSpecificationFactory.stub(x => x.Create(call)).Return(callSpecification);
            }

            public override CallHandler CreateSubjectUnderTest()
            {
                return new CallHandler(CallStack, configuredResults, PropertyHelper, context, callSpecificationFactory);
            } 
        }

        public class When_a_member_is_called : Concern
        {
            object result;

            [Test]
            public void Should_record_call()
            {
                CallStack.received(x => x.Push(call));
            }

            [Test]
            public void Should_update_last_call_handler_on_substitution_context()
            {
                context.received(x => x.LastCallHandler(sut));
            }

            [Test]
            public void Should_return_value_from_configured_results()
            {
                Assert.That(result, Is.EqualTo(valueToReturn));
            }

            public override void Because()
            {
                result = sut.Handle(call);
            }

            public override void Context()
            {
                base.Context();
                configuredResults.stub(x => x.GetResult(call)).Return(valueToReturn);
            }
        }

        public class When_the_return_value_for_the_last_call_is_set : Concern
        {            
            [Test]
            public void Should_remove_the_call_from_those_recorded_and_add_it_to_configured_results()
            {
                configuredResults.received(x => x.SetResult(call, valueToReturn));
            }

            public override void Because()
            {
                sut.LastCallShouldReturn(valueToReturn);
            }

            public override void Context()
            {
                base.Context();
                CallStack.stub(x => x.Pop()).Return(call);
            }
        }

        public class When_told_to_assert_the_next_call_has_been_received : Concern
        {
            object result;
            object defaultForCall;

            [Test]
            public void Should_throw_exception_if_call_has_not_been_received()
            {
                CallStack.received(x => x.ThrowIfCallNotFound(callSpecification));                
            }

            [Test]
            public void Should_not_add_call_to_stack()
            {
                CallStack.did_not_receive(x => x.Push(call));
            }

            [Test]
            public void Should_return_default_for_call()
            {
                Assert.That(result, Is.EqualTo(defaultForCall));
            }

            [Test]
            public void Next_call_should_go_on_stack()
            {
                sut.Handle(call);
                CallStack.received(x => x.Push(call));
            }

            public override void Because()
            {
                sut.AssertNextCallHasBeenReceived();
                result = sut.Handle(call);
            }

            public override void Context()
            {
                base.Context();
                defaultForCall = new object();
                configuredResults.stub(x => x.GetDefaultResultFor(call)).Return(defaultForCall);
            }
        }

        public class When_call_is_a_property_setter : Concern
        {
            private object setValue;
            private ICall propertyGetter;

            [Test]
            public void Should_add_set_value_to_configured_results()
            {
                configuredResults.received(x => x.SetResult(propertyGetter, setValue));
            }

            public override void Because()
            {
                sut.Handle(call);
            }

            public override void Context()
            {
                base.Context();
                setValue = new object();
                propertyGetter = mock<ICall>();
                call.stub(x => x.GetArguments()).Return(new[] { setValue });
                PropertyHelper.stub(x => x.IsCallToSetAReadWriteProperty(call)).Return(true);
                PropertyHelper.stub(x => x.CreateCallToPropertyGetterFromSetterCall(call)).Return(propertyGetter);
            }
        }
    }
}