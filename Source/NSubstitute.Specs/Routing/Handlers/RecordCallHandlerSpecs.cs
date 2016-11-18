using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class RecordCallHandlerSpecs
    {
        public class When_handling_call_to_a_member : ConcernFor<RecordCallHandler>
        {
            ICall _call;
            ICallCollection _callCollection;
            SequenceNumberGenerator _sequenceNumberGenerator;
            RouteAction _result;

            [Test]
            public void Should_record_call()
            {
                _callCollection.received(x => x.Add(_call));
            }

            [Test]
            public void Should_assign_sequence_number()
            {
                _call.received(x => x.AssignSequenceNumber(42));
            }

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue()));
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _callCollection = mock<ICallCollection>();
                _call = mock<ICall>();
                _sequenceNumberGenerator = mock<SequenceNumberGenerator>();
                _sequenceNumberGenerator.stub(x => x.Next()).Return(42);
            }

            public override RecordCallHandler CreateSubjectUnderTest()
            {
                return new RecordCallHandler(_callCollection, _sequenceNumberGenerator);
            }
        }
    }
}