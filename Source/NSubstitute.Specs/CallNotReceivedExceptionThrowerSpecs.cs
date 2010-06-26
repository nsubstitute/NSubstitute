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
        public class When_throwing : ConcernFor<CallNotReceivedExceptionThrower>
        {
            private const string DescriptionOfCall = "SomeSampleMethod(args)";
            private ICallSpecification _callSpecification;
            private CallNotReceivedException _exception;
            private ICallFormatter _callFormatter;
            private IEnumerable<ICall> _actualCalls;
            private const string HowDifferentFromFirst = "First";
            private const string HowDifferentFromSecond = "Second";

            [Test]
            public void Exception_should_contain_description_of_expected_call()
            {
                Assert.That(_exception.Message, Is.StringContaining(DescriptionOfCall));
            }

            [Test]
            public void Exception_should_contain_how_actual_calls_differ()
            {
                Assert.That(_exception.Message, Is.StringContaining(HowDifferentFromFirst)); 
                Assert.That(_exception.Message, Is.StringContaining(HowDifferentFromSecond)); 
            }

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
                _actualCalls = new[] {mock<ICall>(), mock<ICall>() };
                _callSpecification = mock<ICallSpecification>();

                _callFormatter = mock<ICallFormatter>();

                _callSpecification.stub(x => x.Format(_callFormatter)).Return(DescriptionOfCall);
                _callSpecification.stub(x => x.HowDifferentFrom(_actualCalls.First(), _callFormatter)).Return(HowDifferentFromFirst);
                _callSpecification.stub(x => x.HowDifferentFrom(_actualCalls.ElementAt(1), _callFormatter)).Return(HowDifferentFromSecond);
            }

            public override CallNotReceivedExceptionThrower CreateSubjectUnderTest()
            {
                return new CallNotReceivedExceptionThrower(_callFormatter);
            }
        }
    }
}