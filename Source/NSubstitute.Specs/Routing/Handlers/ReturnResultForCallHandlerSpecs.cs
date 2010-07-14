using System;
using NSubstitute.Core;
using NSubstitute.Routes.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes.Handlers
{
    public class ReturnResultForCallHandlerSpecs
    {
        public abstract class When_handling_call : ConcernFor<ReturnResultForCallHandler>
        {
            protected ICallResults _callResults;
            protected IDefaultForType _defaultForType;
            protected ICall _call;
            protected object _result;

            public override void Context()
            {
                _call = mock<ICall>();
                _callResults = mock<ICallResults>();
                _defaultForType = mock<IDefaultForType>();
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }


            public override ReturnResultForCallHandler CreateSubjectUnderTest()
            {
                return new ReturnResultForCallHandler(_callResults, _defaultForType);
            }       
        }

        public class When_handling_call_with_configured_result : When_handling_call
        {
            private readonly object _expectedResult = new object();

            [Test]
            public void Should_return_configured_result()
            {
                Assert.That(_result, Is.SameAs(_expectedResult));
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
            readonly object _defaultForReturnType = new object();
            readonly Type _returnType = typeof(object);

            [Test]
            public void Should_return_default_for_call_return_type()
            {
                Assert.That(_result, Is.SameAs(_defaultForReturnType)); 
            }

            public override void Context()
            {
                base.Context();
                _callResults.stub(x => x.HasResultFor(_call)).Return(false);
                _call.stub(x => x.GetReturnType()).Return(_returnType);
                _defaultForType.stub(x => x.GetDefaultFor(_returnType)).Return(_defaultForReturnType);
            }
        }
    }
}