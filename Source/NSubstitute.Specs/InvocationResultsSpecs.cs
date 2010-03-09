using System;
using NSubstitute.Specs.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class InvocationResultsSpecs
    {
        public abstract class Concern : ConcernFor<InvocationResults>
        {
            protected IInvocationMatcher invocationMatcher;

            public override void Context()
            {
                invocationMatcher = mock<IInvocationMatcher>();
            }

            public override InvocationResults CreateSubjectUnderTest()
            {
                return new InvocationResults(invocationMatcher);
            }
        }

        public class When_getting_a_result_that_has_been_set : Concern
        {
            object result;
            object originalResult;
            IInvocation originalInvocation;
            IInvocation secondInvocation;

            [Test]
            public void Should_get_the_result_that_was_set()
            {
                Assert.That(result, Is.SameAs(originalResult));
            }

            public override void Because()
            {
                sut.SetResult(originalInvocation, originalResult);
                result = sut.GetResult(secondInvocation);
            }

            public override void Context()
            {
                base.Context();
                originalResult = new object();
                originalInvocation = mock<IInvocation>();
                secondInvocation = mock<IInvocation>();
                invocationMatcher.stub(x => x.IsMatch(originalInvocation, secondInvocation)).Return(true);
            }
        }

        public class When_getting_a_reference_type_result_that_has_not_been_set : Concern
        {
            object result;
            IInvocation invocation;

            [Test]
            public void Should_use_the_default_value_for_the_result_type()
            {
                Assert.That(result, Is.Null);
            }

            public override void Because()
            {
                result = sut.GetResult(invocation);
            }

            public override void Context()
            {
                base.Context();
                invocation = mock<IInvocation>();
                invocation.stub(x => x.GetReturnType()).Return(typeof(List));
            }
        }
        
        public class When_getting_a_value_type_result_that_has_not_been_set : Concern
        {
            object result;
            IInvocation invocation;

            [Test]
            public void Should_use_the_default_value_for_the_result_type()
            {
                Assert.That(result, Is.EqualTo(default(int)));
            }

            public override void Because()
            {
                result = sut.GetResult(invocation);
            }

            public override void Context()
            {
                base.Context();
                invocation = mock<IInvocation>();
                invocation.stub(x => x.GetReturnType()).Return(typeof(int));
            }
        }

        public class When_getting_a_void_type_result : Concern
        {
            object result;
            IInvocation invocation;

            [Test]
            public void Should_return_null_because_there_is_no_void_instance()
            {
                Assert.That(result, Is.Null);
            }

            public override void Because()
            {
                result = sut.GetResult(invocation);
            }

            public override void Context()
            {
                base.Context();
                invocation = mock<IInvocation>();
                invocation.stub(x => x.GetReturnType()).Return(typeof (void));
            }
        }

        public class When_getting_default_results : Concern
        {
            [Test]
            public void Should_return_null_for_reference_types()
            {
                var invocationThatReturnsReferenceType = CreateInvocationWithReturnType(typeof(string));
                var result = sut.GetDefaultResultFor(invocationThatReturnsReferenceType);
                Assert.That(result, Is.Null);
            }

            [Test]
            public void Should_return_default_for_value_types()
            {
                var invocationThatReturnsValueType = CreateInvocationWithReturnType(typeof(int));
                var result = sut.GetDefaultResultFor(invocationThatReturnsValueType);
                Assert.That(result, Is.EqualTo(default(int)));
            }

            [Test]
            public void Should_return_null_for_void_type()
            {
                var invocationThatReturnsVoidType = CreateInvocationWithReturnType(typeof (void));
                var result = sut.GetDefaultResultFor(invocationThatReturnsVoidType);
                Assert.That(result, Is.Null);
            }

            IInvocation CreateInvocationWithReturnType(Type type)
            {
                var invocation = mock<IInvocation>();
                invocation.stub(x => x.GetReturnType()).Return(type);
                return invocation;
            }
        }
    }
}