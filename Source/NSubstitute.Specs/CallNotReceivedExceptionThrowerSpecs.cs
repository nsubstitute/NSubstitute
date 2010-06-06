using System.Reflection;
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
            private MethodInfo _methodInfo;
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
                throw new AssertionException("Expected a CallNotFoundException to be throw.");
            }

            public override void Context()
            {
                base.Context();
                _methodInfo = typeof(ISample).GetMethod("SomeSampleMethod");
                _callSpecification = new CallSpecification(_methodInfo);
                _callFormatter = mock<ICallFormatter>();
                _callFormatter.stub(x => x.Format(_methodInfo)).Return(DescriptionOfCall);
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