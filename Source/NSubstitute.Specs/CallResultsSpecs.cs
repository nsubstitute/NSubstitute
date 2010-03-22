using System;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallResultsSpecs
    {
        public abstract class Concern : ConcernFor<CallResults>
        {
            protected ICallMatcher CallMatcher;

            public override void Context()
            {
                CallMatcher = mock<ICallMatcher>();
            }

            public override CallResults CreateSubjectUnderTest()
            {
                return new CallResults(CallMatcher);
            }
        }

        public class When_getting_a_result_that_has_been_set : Concern
        {
            object result;
            object originalResult;
            ICall _originalCall;
            ICall _secondCall;

            [Test]
            public void Should_get_the_result_that_was_set()
            {
                Assert.That(result, Is.SameAs(originalResult));
            }

            public override void Because()
            {
                sut.SetResult(_originalCall, originalResult);
                result = sut.GetResult(_secondCall);
            }

            public override void Context()
            {
                base.Context();
                originalResult = new object();
                _originalCall = mock<ICall>();
                _secondCall = mock<ICall>();
                CallMatcher.stub(x => x.IsMatch(_originalCall, _secondCall)).Return(true);
            }
        }

        public class When_getting_a_reference_type_result_that_has_not_been_set : Concern
        {
            object result;
            ICall call;

            [Test]
            public void Should_use_the_default_value_for_the_result_type()
            {
                Assert.That(result, Is.Null);
            }

            public override void Because()
            {
                result = sut.GetResult(call);
            }

            public override void Context()
            {
                base.Context();
                call = mock<ICall>();
                call.stub(x => x.GetReturnType()).Return(typeof(List));
            }
        }
        
        public class When_getting_a_value_type_result_that_has_not_been_set : Concern
        {
            object result;
            ICall call;

            [Test]
            public void Should_use_the_default_value_for_the_result_type()
            {
                Assert.That(result, Is.EqualTo(default(int)));
            }

            public override void Because()
            {
                result = sut.GetResult(call);
            }

            public override void Context()
            {
                base.Context();
                call = mock<ICall>();
                call.stub(x => x.GetReturnType()).Return(typeof(int));
            }
        }

        public class When_getting_a_void_type_result : Concern
        {
            object result;
            ICall call;

            [Test]
            public void Should_return_null_because_there_is_no_void_instance()
            {
                Assert.That(result, Is.Null);
            }

            public override void Because()
            {
                result = sut.GetResult(call);
            }

            public override void Context()
            {
                base.Context();
                call = mock<ICall>();
                call.stub(x => x.GetReturnType()).Return(typeof (void));
            }
        }

        public class When_getting_default_results : Concern
        {
            [Test]
            public void Should_return_null_for_reference_types()
            {
                var callThatReturnsReferenceType = CreateCallWithReturnType(typeof(string));
                var result = sut.GetDefaultResultFor(callThatReturnsReferenceType);
                Assert.That(result, Is.Null);
            }

            [Test]
            public void Should_return_default_for_value_types()
            {
                var callThatReturnsValueType = CreateCallWithReturnType(typeof(int));
                var result = sut.GetDefaultResultFor(callThatReturnsValueType);
                Assert.That(result, Is.EqualTo(default(int)));
            }

            [Test]
            public void Should_return_null_for_void_type()
            {
                var callThatReturnsVoidType = CreateCallWithReturnType(typeof (void));
                var result = sut.GetDefaultResultFor(callThatReturnsVoidType);
                Assert.That(result, Is.Null);
            }

            ICall CreateCallWithReturnType(Type type)
            {
                var call = mock<ICall>();
                call.stub(x => x.GetReturnType()).Return(type);
                return call;
            }
        }
    }
}