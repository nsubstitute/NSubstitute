using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallActionsSpecs
    {
        public class When_finding_matching_actions_for_a_call : ConcernFor<CallActions>
        {
            private Action<CallInfo> _firstMatchingAction;
            private Action<CallInfo> _secondMatchingAction;
            private Action<CallInfo> _nonMatchingAction;
            private ICallSpecification _matchingCallSpec;
            private ICallSpecification _nonMatchingCallSpec;
            private ICall _call;
            private IEnumerable<Action<CallInfo>> _result;

            [Test]
            public void Should_return_all_actions_that_match_call_specification()
            {
                var results = _result.ToArray();
                Assert.That(results.Length, Is.EqualTo(2), "Expected two matching actions");
                Assert.That(results[0], Is.SameAs(_firstMatchingAction));
                Assert.That(results[1], Is.SameAs(_secondMatchingAction));
            }

            public override void Because()
            {
                sut.Add(_matchingCallSpec, _firstMatchingAction);
                sut.Add(_nonMatchingCallSpec, _nonMatchingAction);
                sut.Add(_matchingCallSpec, _secondMatchingAction);

                _result = sut.MatchingActions(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                
                _matchingCallSpec = mock<ICallSpecification>();
                _matchingCallSpec.stub(x => x.IsSatisfiedBy(_call)).Return(true);

                _nonMatchingCallSpec = mock<ICallSpecification>();
                _firstMatchingAction = x => { };
                _secondMatchingAction = x => { };
                _nonMatchingAction = x => { };
            }

            public override CallActions CreateSubjectUnderTest()
            {
                return new CallActions();
            }
        }
    }
}