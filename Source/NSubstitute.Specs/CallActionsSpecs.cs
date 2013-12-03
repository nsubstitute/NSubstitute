using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallActionsSpecs
    {
        public class Concern : ConcernFor<CallActions>
        {
            protected ICallInfoFactory _callInfoFactory;
            protected ICall _call;
            protected CallInfo _callInfo;

            protected ICallSpecification CreateMatchingCallSpec()
            {
                var callSpec = mock<ICallSpecification>();
                callSpec.stub(x => x.IsSatisfiedBy(_call)).Return(true);
                return callSpec;
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _callInfo = new CallInfo(new Argument[0]);
                _callInfoFactory = mock<ICallInfoFactory>();
                _callInfoFactory.stub(x => x.Create(_call)).Return(_callInfo);
            }

            public override CallActions CreateSubjectUnderTest()
            {
                return new CallActions(_callInfoFactory);
            }
        }

        public class When_invoking_matching_actions_for_a_call : Concern
        {
            private Action<CallInfo> _firstMatchingAction;
            private Action<CallInfo> _secondMatchingAction;
            private Action<CallInfo> _nonMatchingAction;
            private ICallSpecification _firstMatchingCallSpec;
            private ICallSpecification _secondMatchingCallSpec;
            private ICallSpecification _nonMatchingCallSpec;

            [Test]
            public void Should_invoke_all_actions_that_match_call_specification()
            {
                _firstMatchingAction.received(x => x.Invoke(_callInfo));
                _secondMatchingAction.received(x => x.Invoke(_callInfo));
                _nonMatchingAction.did_not_receive(x => x.Invoke(_callInfo));
            }

            [Test]
            public void Should_invoke_per_argument_actions_specified_on_matching_specifications()
            {
                _firstMatchingCallSpec.received(x => x.InvokePerArgumentActions(_callInfo));
                _secondMatchingCallSpec.received(x => x.InvokePerArgumentActions(_callInfo));
                _nonMatchingCallSpec.did_not_receive(x => x.InvokePerArgumentActions(_callInfo));
            }

            public override void Because()
            {
                sut.Add(_firstMatchingCallSpec, _firstMatchingAction);
                sut.Add(_nonMatchingCallSpec, _nonMatchingAction);
                sut.Add(_secondMatchingCallSpec, _secondMatchingAction);

                sut.InvokeMatchingActions(_call);
            }

            public override void Context()
            {
                base.Context();

                _firstMatchingCallSpec = CreateMatchingCallSpec();
                _secondMatchingCallSpec = CreateMatchingCallSpec();

                _nonMatchingCallSpec = mock<ICallSpecification>();
                _firstMatchingAction = mock<Action<CallInfo>>();
                _secondMatchingAction = mock<Action<CallInfo>>();
                _nonMatchingAction = mock<Action<CallInfo>>();
            }
        }

        public class When_moving_actions_for_a_spec_to_a_new_spec : Concern
        {
            private ICallSpecification _initialCallSpec;
            private ICallSpecification _newSpec;
            private Action<CallInfo> _callMatchingInitialSpec;

            [Test]
            public void Should_invoke_based_on_new_spec()
            {
                sut.InvokeMatchingActions(_call);

                _callMatchingInitialSpec.received(x => x.Invoke(_callInfo));
            }

            public override void Because()
            {
                sut.Add(_initialCallSpec, _callMatchingInitialSpec);
                sut.MoveActionsForSpecToNewSpec(_initialCallSpec, _newSpec);
            }

            public override void Context()
            {
                base.Context();
                _initialCallSpec = CreateMatchingCallSpec();
                _newSpec = CreateMatchingCallSpec();
                _callMatchingInitialSpec = mock<Action<CallInfo>>();
            }
        }
    }
}