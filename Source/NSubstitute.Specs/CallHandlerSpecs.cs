using System.Collections.Generic;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallHandlerSpecs
    {
        public abstract class Concern : ConcernFor<CallHandler>
        {
            protected int _valueToReturn;
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected ICallStack _callStack;
            protected ICallResults _configuredResults;
            protected IPropertyHelper _propertyHelper;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;
            protected IList<IArgumentMatcher> _argumentMatchers;

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _callStack = mock<ICallStack>();
                _configuredResults = mock<ICallResults>();
                _propertyHelper = mock<IPropertyHelper>();
                _call = mock<ICall>();
                _argumentMatchers = mock<IList<IArgumentMatcher>>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.Create(_call, _argumentMatchers)).Return(_callSpecification);
            }

            public override CallHandler CreateSubjectUnderTest()
            {
                return new CallHandler(_callStack, _configuredResults, _propertyHelper, _context, _callSpecificationFactory);
            } 
        }

        public class When_a_member_is_called : Concern
        {
            object result;

            [Test]
            public void Should_record_call()
            {
                _callStack.received(x => x.Push(_call));
            }

            [Test]
            public void Should_update_last_call_handler_on_substitution_context()
            {
                _context.received(x => x.LastCallHandler(sut));
            }

            [Test]
            public void Should_return_value_from_configured_results()
            {
                Assert.That(result, Is.EqualTo(_valueToReturn));
            }

            public override void Because()
            {
                result = sut.Handle(_call, _argumentMatchers);
            }

            public override void Context()
            {
                base.Context();
                _configuredResults.stub(x => x.GetResult(_call)).Return(_valueToReturn);
            }
        }

        public class When_the_return_value_for_the_last_call_is_set : Concern
        {            
            [Test]
            public void Should_remove_the_call_from_those_recorded_and_add_it_to_configured_results()
            {
                _configuredResults.received(x => x.SetResult(_callSpecification, _valueToReturn));
            }

            public override void Because()
            {
                sut.LastCallShouldReturn(_valueToReturn, _argumentMatchers);
            }

            public override void Context()
            {
                base.Context();
                _callStack.stub(x => x.Pop()).Return(_call);
            }
        }

        public class When_told_to_assert_the_next_call_has_been_received : Concern
        {
            object result;
            object defaultForCall;

            [Test]
            public void Should_throw_exception_if_call_has_not_been_received()
            {
                _callStack.received(x => x.ThrowIfCallNotFound(_callSpecification));                
            }

            [Test]
            public void Should_not_add_call_to_stack()
            {
                _callStack.did_not_receive(x => x.Push(_call));
            }

            [Test]
            public void Should_return_default_for_call()
            {
                Assert.That(result, Is.EqualTo(defaultForCall));
            }

            [Test]
            public void Next_call_should_go_on_stack()
            {
                sut.Handle(_call, new List<IArgumentMatcher>());
                _callStack.received(x => x.Push(_call));
            }

            public override void Because()
            {
                sut.AssertNextCallHasBeenReceived();
                result = sut.Handle(_call, _argumentMatchers);
            }

            public override void Context()
            {
                base.Context();
                defaultForCall = new object();
                _configuredResults.stub(x => x.GetDefaultResultFor(_call)).Return(defaultForCall);
            }
        }

        public class When_call_is_a_property_setter : Concern
        {
            private object setValue;
            private ICall propertyGetter;
            private ICallSpecification propertyGetterSpecification;

            [Test]
            public void Should_add_set_value_to_configured_results()
            {
                _configuredResults.received(x => x.SetResult(propertyGetterSpecification, setValue));
            }

            public override void Because()
            {
                sut.Handle(_call, _argumentMatchers);
            }

            public override void Context()
            {
                base.Context();
                setValue = new object();
                propertyGetter = mock<ICall>();
                propertyGetterSpecification = mock<ICallSpecification>();
                _call.stub(x => x.GetArguments()).Return(new[] { setValue });
                _propertyHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(true);
                _propertyHelper.stub(x => x.CreateCallToPropertyGetterFromSetterCall(_call)).Return(propertyGetter);
                _callSpecificationFactory.stub(x => x.Create(propertyGetter, _argumentMatchers)).Return(propertyGetterSpecification);
            }
        }
    }
}