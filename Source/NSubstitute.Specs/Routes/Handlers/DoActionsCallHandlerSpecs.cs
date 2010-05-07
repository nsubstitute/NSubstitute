using System;
using NSubstitute.Core;
using NSubstitute.Routes.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes.Handlers
{
    public class DoActionsCallHandlerSpecs
    {
        public class When_handling_a_call : ConcernFor<DoActionsCallHandler>
        {
            private ICall _call;
            private ICallActions _callActions;
            private Action<object[]>[] _matchingActions;
            private object[] _callArguments;

            [Test]
            public void Should_invoke_all_actions_for_call()
            {
                foreach (var matchingAction in _matchingActions)
                {
                    matchingAction.received(x => x(_callArguments));
                }
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                _matchingActions = new[] { mock<Action<object[]>>(), mock<Action<object[]>>() };
                _callArguments = new[] {new object(), new object()};
                
                _call = mock<ICall>();
                _callActions = mock<ICallActions>();

                _call.stub(x => x.GetArguments()).Return(_callArguments);
                _callActions.stub(x => x.MatchingActions(_call)).Return(_matchingActions);
            }

            public override DoActionsCallHandler CreateSubjectUnderTest()
            {
                return new DoActionsCallHandler(_callActions);
            }
        }
    }
}