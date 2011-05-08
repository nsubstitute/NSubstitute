using System;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class DoActionsCallHandlerSpecs
    {
        public class When_handling_a_call : ConcernFor<DoActionsCallHandler>
        {
            private CallInfo _callInfo;
            private ICall _call;            
            private ICallActions _callActions;
            private Action<CallInfo>[] _matchingActions;
            private ICallInfoFactory _callInfoFactory;
            private RouteAction _result;

            [Test]
            public void Should_invoke_all_actions_for_call_with_call_information()
            {
                foreach (var matchingAction in _matchingActions)
                {
                    matchingAction.received(x => x(_callInfo));
                }
            }

            [Test]
            public void Should_continue_route()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue())); 
            }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _matchingActions = new[] { mock<Action<CallInfo>>(), mock<Action<CallInfo>>() };
                _callInfo = new CallInfo(new Argument[0]);

                _call = mock<ICall>();
                _callActions = mock<ICallActions>();
                _callInfoFactory = mock<ICallInfoFactory>();

                _callActions.stub(x => x.MatchingActions(_call)).Return(_matchingActions);
                _callInfoFactory.stub(x => x.Create(_call)).Return(_callInfo);
            }

            public override DoActionsCallHandler CreateSubjectUnderTest()
            {
                return new DoActionsCallHandler(_callActions, _callInfoFactory);
            }
        }
    }
}