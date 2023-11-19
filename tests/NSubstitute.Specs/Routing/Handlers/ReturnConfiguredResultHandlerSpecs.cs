using System;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class ReturnConfiguredResultHandlerSpecs
    {
        public abstract class When_handling_call : ConcernFor<ReturnConfiguredResultHandler>
        {
            protected ICallResults _callResults;
            protected ICall _call;
            protected RouteAction _result;

            public override void Context()
            {
                _call = mock<ICall>();
                _callResults = mock<ICallResults>();
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override ReturnConfiguredResultHandler CreateSubjectUnderTest()
            {
                return new ReturnConfiguredResultHandler(_callResults);
            }       
        }

        public class When_handling_call_with_configured_result : When_handling_call
        {
            private readonly object _expectedResult = new object();

            [Test]
            public void Should_return_configured_result()
            {
                Assert.That(_result.ReturnValue, Is.SameAs(_expectedResult));
            }

            public override void Context()
            {
                base.Context();
                _callResults.stub(x => x.HasResultFor(_call)).Return(true);
                _callResults.stub(x => x.GetResult(_call)).Return(_expectedResult);
            }
        }

        public class When_handling_call_without_configured_result : When_handling_call
        {
            readonly Type _returnType = typeof(object);

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue()));
            }

            public override void Context()
            {
                base.Context();
                _callResults.stub(x => x.HasResultFor(_call)).Return(false);
                _call.stub(x => x.GetReturnType()).Return(_returnType);
            }
        }
    }
}