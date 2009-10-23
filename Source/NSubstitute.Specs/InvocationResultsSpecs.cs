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
    }
}