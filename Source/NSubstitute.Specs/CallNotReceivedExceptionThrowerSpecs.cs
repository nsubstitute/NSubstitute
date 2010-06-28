using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallNotReceivedExceptionThrowerSpecs
    {
        public class Concern : ConcernFor<CallNotReceivedExceptionThrower>
        {
            protected const string DescriptionOfCall = "SomeSampleMethod(args)";
            protected ICallSpecification _callSpecification;
            protected CallNotReceivedException _exception;
            protected ICallFormatter _callFormatter;
            protected IEnumerable<ICall> _actualCalls;

            public override void Because()
            {
                try
                {
                    sut.Throw(_callSpecification, _actualCalls);
                }
                catch (CallNotReceivedException ex)
                {
                    _exception = ex;
                    return;
                }
                throw new AssertionException("Expected a CallNotFoundException to be thrown.");
            }

            public override void Context()
            {
                base.Context();
                _callSpecification = mock<ICallSpecification>();
                _callFormatter = mock<ICallFormatter>();
                _callSpecification.stub(x => x.Format(_callFormatter)).Return(DescriptionOfCall);
            }

            public override CallNotReceivedExceptionThrower CreateSubjectUnderTest()
            {
                return new CallNotReceivedExceptionThrower(_callFormatter);
            }
        }

        public class When_throwing_exception_with_actual_calls_to_specified_method : Concern
        {
            const string FormattedFirstCall = "First";
            const string FormattedSecondCall = "Second";

            [Test]
            public void Exception_should_contain_description_of_expected_call()
            {
                Assert.That(_exception.Message, Is.StringContaining(DescriptionOfCall));
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
                _callFormatter.stub(x => x.Format(_actualCalls.First())).Return(FormattedFirstCall);
                _callFormatter.stub(x => x.Format(_actualCalls.ElementAt(1))).Return(FormattedSecondCall);
            }
        }

        public class When_throwing_exception_with_no_actual_calls : Concern
        {

            [Test]
            public void Exception_should_contain_description_of_expected_call()
            {
                Assert.That(_exception.Message, Is.StringContaining(DescriptionOfCall));
            }

            [Test]
            public void Exception_should_state_no_calls_to_member_received()
            {
                Assert.That(_exception.Message, Is.StringContaining("Actually received no calls that ressemble the expected call."));
            }

            public override void Context()
            {
                base.Context();
                _actualCalls = new ICall[0];
            }
        }
    }
}