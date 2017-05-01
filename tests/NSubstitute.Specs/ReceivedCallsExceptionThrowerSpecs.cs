using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ReceivedCallsExceptionThrowerSpecs
    {
        public class Concern : ConcernFor<ReceivedCallsExceptionThrower>
        {
            protected string _descriptionOfCall;
            protected ICallSpecification _callSpecification;
            protected ReceivedCallsException _exception;
            protected IEnumerable<ICall> _actualCalls;
            protected IEnumerable<ICall> _relatedCalls;
            protected Quantity _requiredQuantity = Quantity.AtLeastOne();

            public override void Because()
            {
                try
                {
                    _requiredQuantity = Quantity.AtLeastOne();
                    sut.Throw(_callSpecification, _actualCalls, _relatedCalls, _requiredQuantity);
                }
                catch (ReceivedCallsException ex)
                {
                    _exception = ex;
                    return;
                }
                throw new AssertionException("Expected a matching exception to be thrown.");
            }

            public override void Context()
            {
                base.Context();
                _callSpecification = mock<ICallSpecification>();
                _relatedCalls = new ICall[0];
                _descriptionOfCall = _callSpecification.ToString();
            }

            public override ReceivedCallsExceptionThrower CreateSubjectUnderTest()
            {
                return new ReceivedCallsExceptionThrower();
            }
        }

        public class When_throwing_exception_with_actual_calls_to_specified_method : Concern
        {
            const string FormattedFirstCall = "First";
            const string FormattedSecondCall = "Second";

            [Test]
            public void Exception_should_contain_description_of_expected_call()
            {
                Assert.That(_exception.Message, Is.StringContaining(_descriptionOfCall));
            }

            [Test]
            public void Exception_should_list_actual_calls()
            {
                Assert.That(_exception.Message, Is.StringContaining(FormattedFirstCall)); 
                Assert.That(_exception.Message, Is.StringContaining(FormattedSecondCall)); 
            }

            public override void Context()
            {
                base.Context();
                _actualCalls = new[] {mock<ICall>(), mock<ICall>() };
                _callSpecification.stub(x => x.Format(_actualCalls.First())).Return(FormattedFirstCall);
                _callSpecification.stub(x => x.Format(_actualCalls.ElementAt(1))).Return(FormattedSecondCall);
            }
        }

        public class When_throwing_exception_with_no_actual_calls : Concern
        {

            [Test]
            public void Exception_should_contain_description_of_expected_call()
            {
                Assert.That(_exception.Message, Is.StringContaining(_descriptionOfCall));
            }

            [Test]
            public void Exception_should_state_no_calls_to_member_received()
            {
                Assert.That(_exception.Message, Is.StringContaining("Actually received no matching calls."));
            }

            public override void Context()
            {
                base.Context();
                _actualCalls = new ICall[0];
            }
        }
    }
}