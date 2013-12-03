using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallResultsSpecs
    {
        public abstract class Concern : ConcernFor<CallResults>
        {
            private ICallInfoFactory _callInfoFactory;

            public override void Context()
            {
                _callInfoFactory = mock<ICallInfoFactory>();
            }

            public override CallResults CreateSubjectUnderTest()
            {
                return new CallResults(_callInfoFactory);
            }

            protected CallInfo StubCallInfoForCall(ICall call)
            {
                var callInfo = new CallInfo(new Argument[0]);
                _callInfoFactory.stub(x => x.Create(call)).Return(callInfo);
                return callInfo;
            }
        }

        public class When_a_result_that_has_been_set : Concern
        {
            readonly object _expectedResult = new object();
            ICallSpecification _callSpecification;
            ICall _call;
            IReturn _resultToReturn;

            [Test]
            public void Should_have_result_for_call()
            {
                Assert.That(sut.HasResultFor(_call)); 
            }
            [Test]
            public void Should_get_the_result_that_was_set()
            {
                Assert.That(sut.GetResult(_call), Is.SameAs(_expectedResult));
            }

            public override void Because()
            {
                sut.SetResult(_callSpecification, _resultToReturn);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
                _callSpecification.stub(x => x.IsSatisfiedBy(_call)).Return(true);
                
                var callInfo = StubCallInfoForCall(_call);
                _resultToReturn = mock<IReturn>();
                _resultToReturn.stub(x => x.ReturnFor(callInfo)).Return(_expectedResult);
            }
        }

        public class When_getting_a_result_that_has_been_set_multiple_times : Concern
        {
            object _result;
            readonly object _originalResult = new object();
            readonly object _overriddenResult = new object();
            IReturn _returnOriginalResult;
            IReturn _returnOverriddenResult;
            ICallSpecification _callSpecification;
            ICall _call;

            [Test]
            public void Should_get_the_last_result_that_was_set()
            {
                Assert.That(_result, Is.SameAs(_overriddenResult));
            }

            public override void Because()
            {
                sut.SetResult(_callSpecification, _returnOriginalResult);
                sut.SetResult(_callSpecification, _returnOverriddenResult);
                _result = sut.GetResult(_call);
            }

            public override void Context()
            {
                base.Context();
                _callSpecification = mock<ICallSpecification>();
                _call = mock<ICall>();
                _callSpecification.stub(x => x.IsSatisfiedBy(_call)).Return(true);
                var callInfo = StubCallInfoForCall(_call);
                _returnOriginalResult = mock<IReturn>();
                _returnOriginalResult.stub(x => x.ReturnFor(callInfo)).Return(_originalResult);
                _returnOverriddenResult = mock<IReturn>();
                _returnOverriddenResult.stub(x => x.ReturnFor(callInfo)).Return(_overriddenResult);
            }
            
        }

        public class When_a_result_has_not_been_set_for_a_call : Concern
        {
            private ICall _call;

            [Test]
            public void Should_not_have_result_for_call()
            {
                Assert.That(sut.HasResultFor(_call), Is.False);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
            }
        }

        public class When_getting_a_void_type_result : Concern
        {
            ICall _call;

            [Test]
            public void Should_not_have_result_for_call()
            {
                Assert.That(sut.HasResultFor(_call), Is.False);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
                _call.stub(x => x.GetReturnType()).Return(typeof (void));
            }
        }
    }
}