using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class InvocationResultsSpecs
    {
        public abstract class Concern : ConcernFor<InvocationResults>
        {
            protected object actualResult;
            protected IInvocation invocation;

            public override void Context()
            {
                invocation = mock<IInvocation>();
            }

            public override InvocationResults CreateSubjectUnderTest()
            {
                return new InvocationResults();
            }
        }

        public class When_getting_a_result_that_has_been_set : Concern
        {
            protected object originalResult;

            [Test]
            public void Should_get_the_result_that_was_set()
            {
                Assert.That(actualResult, Is.SameAs(originalResult));
            }

            public override void Because()
            {
                sut.SetResult(invocation, originalResult);
                actualResult = sut.GetResult(invocation);
            }

            public override void Context()
            {
                base.Context();
                originalResult = new object();            
            }
        }

        public class When_getting_a_reference_type_result_that_has_not_been_set : Concern
        {
            object result;

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
                invocation.stub(x => x.GetReturnType()).Return(typeof(List));
            }
        }
        
        public class When_getting_a_value_type_result_that_has_not_been_set : Concern
        {
            object result;

            [Test]
            public void Should_use_the_default_value_for_the_result_type()
            {
                Assert.That(result, Is.EqualTo(0));
            }

            public override void Because()
            {
                result = sut.GetResult(invocation);
            }

            public override void Context()
            {
                base.Context();
                invocation.stub(x => x.GetReturnType()).Return(typeof(int));
            }
        }
    }
}