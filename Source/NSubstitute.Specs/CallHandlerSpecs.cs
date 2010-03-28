using System;
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
            protected IReflectionHelper _reflectionHelper;
            protected ICallSpecification _callSpecification;
            protected ICallSpecificationFactory _callSpecificationFactory;
            protected IList<IArgumentMatcher> _argumentMatchers;

            public override void Context()
            {
                _valueToReturn = 7;
                _context = mock<ISubstitutionContext>();
                _callStack = mock<ICallStack>();
                _configuredResults = mock<ICallResults>();
                _reflectionHelper = mock<IReflectionHelper>();
                _call = mock<ICall>();
                _argumentMatchers = mock<IList<IArgumentMatcher>>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.Create(_call, _argumentMatchers)).Return(_callSpecification);
            }

            public override CallHandler CreateSubjectUnderTest()
            {
                return new CallHandler(_callStack, _configuredResults, _reflectionHelper, _context, _callSpecificationFactory);
            } 
        }

        public class When_a_member_is_called : Concern
        {
            object _result;

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
                Assert.That(_result, Is.EqualTo(_valueToReturn));
            }

            public override void Because()
            {
                _result = sut.Handle(_call, _argumentMatchers);
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
            object _result;
            object _defaultForCall;

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
                Assert.That(_result, Is.EqualTo(_defaultForCall));
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
                _result = sut.Handle(_call, _argumentMatchers);
            }

            public override void Context()
            {
                base.Context();
                _defaultForCall = new object();
                _configuredResults.stub(x => x.GetDefaultResultFor(_call)).Return(_defaultForCall);
            }
        }

        public class When_call_is_a_property_setter : Concern
        {
            private object _setValue;
            private ICall _propertyGetter;
            private ICallSpecification _propertyGetterSpecification;

            [Test]
            public void Should_add_set_value_to_configured_results()
            {
                _configuredResults.received(x => x.SetResult(_propertyGetterSpecification, _setValue));
            }

            public override void Because()
            {
                sut.Handle(_call, _argumentMatchers);
            }

            public override void Context()
            {
                base.Context();
                _setValue = new object();
                _propertyGetter = mock<ICall>();
                _propertyGetterSpecification = mock<ICallSpecification>();
                _call.stub(x => x.GetArguments()).Return(new[] { _setValue });
                _reflectionHelper.stub(x => x.IsCallToSetAReadWriteProperty(_call)).Return(true);
                _reflectionHelper.stub(x => x.CreateCallToPropertyGetterFromSetterCall(_call)).Return(_propertyGetter);
                _callSpecificationFactory.stub(x => x.Create(_propertyGetter, _argumentMatchers)).Return(_propertyGetterSpecification);
            }
        }

        public class When_told_to_raise_when_from_next_call :Concern
        {
            private ICall _eventAssignment;
            private object[] _arguments;

            [Test]
            public void Should_raise_event_from_reference_using_reflection()
            {
                _reflectionHelper.received(x => x.RaiseEventFromEventAssignment(_eventAssignment, _arguments));
            }

            [Test]
            public void Should_not_record_call()
            {
                _callStack.did_not_receive(x => x.Push(_eventAssignment));
            }

            [Test]
            public void Should_not_raise_event_on_subsequent_calls()
            {
                var newAssignment = mock<ICall>();
                sut.Handle(newAssignment, null);
                _reflectionHelper.did_not_receive(x => x.RaiseEventFromEventAssignment(newAssignment, _arguments));

            }
            public override void Because()
            {
                sut.RaiseEventFromNextCall(_arguments);
                sut.Handle(_eventAssignment, null);
            }

            public override void Context()
            {
                base.Context();
                _arguments = new object[0];
                _eventAssignment = mock<ICall>();
            }
        }
    }
}