using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class PendingSpecificationSpecs
    {
        public abstract class Concern : ConcernFor<PendingSpecification>
        {
            protected ISubstitutionContext _substitutionContext;

            public override void Context()
            {
                _substitutionContext = mock<ISubstitutionContext>();
            }

            public override PendingSpecification CreateSubjectUnderTest() { return new PendingSpecification(_substitutionContext); }
        }

        public class When_no_pending_specification : Concern
        {
            [Test]
            public void Should_not_have_a_spec()
            {
                Assert.That(sut.HasPendingCallSpecInfo(), Is.False);
            }

            [Test]
            public void Should_return_null_when_trying_to_use_spec()
            {
                Assert.That(sut.UseCallSpecInfo(), Is.Null);
            }

            public override void Because()
            {
                base.Because();
                _substitutionContext.PendingSpecificationInfo = null;
            }
        }

        public class When_a_pending_specification_has_been_set : Concern
        {
            ICallSpecification _callSpec;

            [Test]
            public void Should_have_a_spec()
            {
                Assert.That(sut.HasPendingCallSpecInfo(), Is.True);
            }

            [Test]
            public void Should_return_the_spec_when_using_it()
            {
                var specInfo = sut.UseCallSpecInfo();
                var result = specInfo.Handle(x => x, x => { throw new Exception("Expected call spec, got last call"); });
                Assert.That(result, Is.SameAs(_callSpec));
            }

            [Test]
            public void Should_not_have_spec_after_it_is_used()
            {
                sut.UseCallSpecInfo();
                Assert.That(sut.HasPendingCallSpecInfo(), Is.False);
            }

            [Test]
            public void Should_not_have_a_spec_after_it_is_cleared()
            {
                sut.Clear();
                Assert.That(sut.HasPendingCallSpecInfo(), Is.False);
            }

            public override void Because()
            {
                base.Because();
                sut.SetCallSpecification(_callSpec);
            }

            public override void Context()
            {
                base.Context();
                _callSpec = mock<ICallSpecification>();
            }
        }

        public class When_a_last_call_info_has_been_set : Concern
        {
            ICall _call;

            [Test]
            public void Should_have_a_spec()
            {
                Assert.That(sut.HasPendingCallSpecInfo(), Is.True);
            }

            [Test]
            public void Should_return_the_call_when_using_it()
            {
                var specInfo = sut.UseCallSpecInfo();
                var result = specInfo.Handle(x => { throw new Exception("Expected last call, got call spec"); }, x => x);
                Assert.That(result, Is.SameAs(_call));
            }

            public override void Because()
            {
                base.Because();
                sut.SetLastCall(_call);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
            }
        }
    }
}