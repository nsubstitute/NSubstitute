using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallRouterSpecs
    {
        public abstract class Concern : ConcernFor<CallRouter>
        {
            protected ISubstitutionContext _context;
            protected ICall _call;
            protected ICallHandler _recordingCallHandler;
            protected ICallHandler _propertySetterHandler;
            protected ICallHandler _checkReceivedCallHandler;
            protected ICallHandler _eventSubscriptionHandler;
            protected IResultSetter _resultSetter;

            public override void Context()
            {
                _context = mock<ISubstitutionContext>();
                _recordingCallHandler = mock<ICallHandler>();
                _checkReceivedCallHandler = mock<ICallHandler>();
                _propertySetterHandler = mock<ICallHandler>();
                _eventSubscriptionHandler = mock<ICallHandler>();
                _call = mock<ICall>();
                _resultSetter = mock<IResultSetter>();
            }

            public override CallRouter CreateSubjectUnderTest()
            {
                return new CallRouter(_context, _recordingCallHandler, _propertySetterHandler, _eventSubscriptionHandler, _checkReceivedCallHandler, _resultSetter);
            } 
        }

        public class When_a_member_is_called : Concern
        {
            object _result;
            int _returnValueFromRecordingHandler;

            [Test]
            public void Should_update_last_call_router_on_substitution_context()
            {
                _context.received(x => x.LastCallRouter(sut));
            }

            [Test]
            public void Should_send_call_to_property_setter()
            {
                _propertySetterHandler.received(x => x.Handle(_call));
            }

            [Test]
            public void Should_send_call_to_event_subscription()
            {
                _eventSubscriptionHandler.received(x => x.Handle(_call));
            }

            [Test]
            public void Should_record_call_and_return_value_from_handler()
            {
                Assert.That(_result, Is.EqualTo(_returnValueFromRecordingHandler));
            }

            public override void Because()
            {
                _result = sut.Route(_call);
            }

            public override void Context()
            {
                base.Context();
                _returnValueFromRecordingHandler = 12;
                _recordingCallHandler.stub(x => x.Handle(_call)).Return(_returnValueFromRecordingHandler);
            }
        }

        public class When_setting_result_of_last_call : Concern
        {
            const int _valueToReturn = 7;

            [Test]
            public void Should_set_result()
            {
                _resultSetter.received(x => x.SetResultForLastCall(_valueToReturn));
            }

            public override void Because()
            {
                sut.LastCallShouldReturn(_valueToReturn);
            }
        }

        public class When_told_to_assert_the_next_call_has_been_received : Concern
        {
            object _result;
            object _valueFromCheckReceivedHandler;

            [Test]
            public void Should_check_call_was_received_and_return_value_from_handler()
            {
                Assert.That(_result, Is.EqualTo(_valueFromCheckReceivedHandler));
            }

            [Test]
            public void Next_call_should_be_recorded()
            {
                sut.Route(_call);
                _recordingCallHandler.received(x => x.Handle(_call));
            }

            public override void Because()
            {
                sut.AssertNextCallHasBeenReceived();
                _result = sut.Route(_call);
            }

            public override void Context()
            {
                base.Context();
                _valueFromCheckReceivedHandler = new object();
                _checkReceivedCallHandler.stub(x => x.Handle(_call)).Return(_valueFromCheckReceivedHandler);
            }
        }
    }
}