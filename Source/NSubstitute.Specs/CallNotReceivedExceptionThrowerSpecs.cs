using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using System.Linq;

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

            [Test]
            public void Exception_should_contain_description_of_expected_call()
            {
                Assert.That(_exception.Message, Is.StringContaining(DescriptionOfCall));
            }

            public override void Because()
            {
                try
                {
                    sut.Throw(_callSpecification);
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
                var methodInfo = typeof(ISample).GetMethod("SomeSampleMethod");
                var argumentSpecifications = new[] { mock<IArgumentSpecification>() };
                _callSpecification = new CallSpecification(methodInfo, argumentSpecifications);

                _callFormatter = mock<ICallFormatter>();
                _callFormatter.stub(x => x.Format(methodInfo, argumentSpecifications)).Return(DescriptionOfCall);
            }

            public override CallNotReceivedExceptionThrower CreateSubjectUnderTest()
            {
                return new CallNotReceivedExceptionThrower(_callFormatter);
            }

            private interface ISample
            {
                void SomeSampleMethod(int a, string b);
            }
        }
    }
}