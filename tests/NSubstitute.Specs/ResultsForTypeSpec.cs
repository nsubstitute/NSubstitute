using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ResultsForTypeSpec
    {
        public abstract class Concern : ConcernFor<ResultsForType>
        {
            private ICallInfoFactory _callInfoFactory;

            public override void Context()
            {
                _callInfoFactory = mock<ICallInfoFactory>();
            }

            public override ResultsForType CreateSubjectUnderTest()
            {
                return new ResultsForType(_callInfoFactory);
            }

            protected CallInfo StubCallInfoForCall(ICall call)
            {
                var callInfo = new CallInfo(new Argument[0]);
                _callInfoFactory.stub(x => x.Create(call)).Return(callInfo);
                return callInfo;
            }
        }

        public class When_a_result_has_not_been_set : Concern
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
                _call = this.mock<ICall>();
            }
        }

        public class When_a_result_has_been_set : Concern
        {
            private readonly object _expectedResult = new object();
            private ICall _call;
            private IReturn _resultToReturn;

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
                sut.SetResult(typeof(IFoo), _resultToReturn);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
                _call.stub(x => x.GetReturnType())
                     .Return(typeof(IFoo));

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
            ICall _call;

            [Test]
            public void Should_get_the_last_result_that_was_set()
            {
                Assert.That(_result, Is.SameAs(_overriddenResult));
            }

            public override void Because()
            {
                sut.SetResult(typeof(IFoo), _returnOriginalResult);
                sut.SetResult(typeof(IFoo), _returnOverriddenResult);
                _result = sut.GetResult(_call);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
                _call.stub(x => x.GetReturnType())
                     .Return(typeof(IFoo));
                var callInfo = StubCallInfoForCall(_call);
                _returnOriginalResult = mock<IReturn>();
                _returnOriginalResult.stub(x => x.ReturnFor(callInfo)).Return(_originalResult);
                _returnOverriddenResult = mock<IReturn>();
                _returnOverriddenResult.stub(x => x.ReturnFor(callInfo)).Return(_overriddenResult);
            }
            
        }

        public class When_type_result_is_void : Concern
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