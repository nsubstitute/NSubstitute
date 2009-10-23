using System.Collections.Generic;
using NSubstitute.Specs.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class AllInvocationMatcherSpecs
    {
        public class Concern : ConcernFor<AllInvocationMatcher>
        {
            protected bool invocationsMatch;
            protected IInvocation firstInvocation;
            protected IInvocation secondInvocation;
            protected IEnumerable<IInvocationMatcher> matchers;

            public override void Because()
            {
                invocationsMatch = sut.IsMatch(firstInvocation, secondInvocation);
            }

            public override void Context()
            {
                firstInvocation = mock<IInvocation>();
                secondInvocation = mock<IInvocation>();
            }

            public override AllInvocationMatcher CreateSubjectUnderTest()
            {
                return new AllInvocationMatcher(matchers);
            }

            protected IInvocationMatcher CreateAlwaysMatchingMatcher()
            {
                var matcher = mock<IInvocationMatcher>();
                matcher.stub(x => x.IsMatch(firstInvocation, secondInvocation)).Return(true);
                return matcher;
            }

            protected IInvocationMatcher CreateNonMatchingMatcher()
            {
                return mock<IInvocationMatcher>();
            }
        }

        public class When_invocations_meet_all_matchers : Concern
        {
            [Test]
            public void Should_return_that_invocations_match()
            {
                Assert.That(invocationsMatch);
            }

            public override void Context()
            {
                base.Context();
                matchers = new[] {CreateAlwaysMatchingMatcher(), CreateAlwaysMatchingMatcher()};
            }

        }

        public class When_invocations_do_not_meet_all_matchers : Concern
        {
            [Test]
            public void Should_not_match_invocations()
            {
                Assert.That(invocationsMatch, Is.False);
            }

            public override void Context()
            {
                base.Context();
                matchers = new[] { CreateAlwaysMatchingMatcher(), CreateNonMatchingMatcher(), CreateAlwaysMatchingMatcher()};
            }
        }
    }
}