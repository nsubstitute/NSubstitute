using System.Collections.Generic;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class AllCallMatcherSpecs
    {
        public class Concern : ConcernFor<AllCallMatcher>
        {
            protected bool callsMatch;
            protected ICall _firstCall;
            protected ICallSpecification _callSpecification;
            protected IEnumerable<ICallMatcher> matchers;

            public override void Because()
            {
                callsMatch = sut.IsMatch(_firstCall, _callSpecification);
            }

            public override void Context()
            {
                _firstCall = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
            }

            public override AllCallMatcher CreateSubjectUnderTest()
            {
                return new AllCallMatcher(matchers);
            }

            protected ICallMatcher CreateAlwaysMatchingMatcher()
            {
                var matcher = mock<ICallMatcher>();
                matcher.stub(x => x.IsMatch(_firstCall, _callSpecification)).Return(true);
                return matcher;
            }

            protected ICallMatcher CreateNonMatchingMatcher()
            {
                return mock<ICallMatcher>();
            }
        }

        public class When_calls_meet_all_matchers : Concern
        {
            [Test]
            public void Should_return_that_calls_match()
            {
                Assert.That(callsMatch);
            }

            public override void Context()
            {
                base.Context();
                matchers = new[] {CreateAlwaysMatchingMatcher(), CreateAlwaysMatchingMatcher()};
            }

        }

        public class When_calls_do_not_meet_all_matchers : Concern
        {
            [Test]
            public void Should_not_match_calls()
            {
                Assert.That(callsMatch, Is.False);
            }

            public override void Context()
            {
                base.Context();
                matchers = new[] { CreateAlwaysMatchingMatcher(), CreateNonMatchingMatcher(), CreateAlwaysMatchingMatcher()};
            }
        }
    }
}