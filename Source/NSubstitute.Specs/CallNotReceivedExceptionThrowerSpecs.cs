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
            private ICallSpecification _callSpecification;
            private CallNotReceivedException _exception;
            private MethodInfo _methodInfo;

            [Test]
            public void Exception_should_contain_name_of_expected_member()
            {
                Assert.That(_exception.Message, Text.Contains(_methodInfo.Name));
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
            }

            public override CallNotReceivedExceptionThrower CreateSubjectUnderTest()
            {
                return new CallNotReceivedExceptionThrower();
            }

            private interface ISample
            {
                void SomeSampleMethod(int a, string b);
            }
        }
    }
}