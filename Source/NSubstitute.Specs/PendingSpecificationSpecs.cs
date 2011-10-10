using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class PendingSpecificationSpecs
    {
        public abstract class Concern : ConcernFor<PendingSpecification>
        {
            public override PendingSpecification CreateSubjectUnderTest() { return new PendingSpecification(); }
        }

        public class When_no_pending_specification : Concern
        {
            [Test]
            public void Should_not_have_a_spec()
            {
                Assert.That(sut.HasPendingCallSpec(), Is.False);
            }

            [Test]
            public void Should_return_null_when_trying_to_use_spec()
            {
                Assert.That(sut.UseCallSpec(), Is.Null);
            }
        }

        public class When_a_pending_specification_has_been_set : Concern
        {
            ICallSpecification _callSpec;

            [Test]
            public void Should_have_a_spec()
            {
                Assert.That(sut.HasPendingCallSpec(), Is.True);
            }

            [Test]
            public void Should_return_the_spec_when_using_it()
            {
                Assert.That(sut.UseCallSpec(), Is.SameAs(_callSpec));
            }

            [Test]
            public void Should_not_have_spec_after_it_is_used()
            {
                sut.UseCallSpec();
                Assert.That(sut.HasPendingCallSpec(), Is.False);
            }

            [Test]
            public void Should_not_have_a_spec_after_it_is_cleared()
            {
                sut.Clear();
                Assert.That(sut.HasPendingCallSpec(), Is.False);
            }

            public override void Because()
            {
                sut.Set(_callSpec);
            }

            public override void Context()
            {
                _callSpec = mock<ICallSpecification>();
            }
        }
    }
}