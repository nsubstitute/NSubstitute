using NSubstitute.Core;
using NSubstitute.Routes.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes.Handlers
{
    public class ReturnConfiguredResultHandlerSpecs
    {
        public class When_handling_call : ConcernFor<ReturnConfiguredResultHandler>
        {
            private ICallResults _callResults;
            private object _result;
            private readonly object _expectedResult = new object();
            private ICall _call;

            [Test]
            public void Should_return_configured_result()
            {
                Assert.That(_result, Is.SameAs(_expectedResult));
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _callResults = mock<ICallResults>();
                _callResults.stub(x => x.GetResult(_call)).Return(_expectedResult);
            }

            public override ReturnConfiguredResultHandler CreateSubjectUnderTest()
            {
                return new ReturnConfiguredResultHandler(_callResults);
            }
        }
    }
}