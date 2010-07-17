using System;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class SetActionsForCallHandlerSpecs
    {
        public class When_handling_a_call : ConcernFor<SetActionForCallHandler>
        {
            private ICallActions _callActions;
            private Action<CallInfo> _action;
            private ICall _call;
            private ICallSpecificationFactory _callSpecificationFactory;
            private ICallSpecification _callSpec;
            private MatchArgs _matchArgs;
            private RouteAction _result;

            [Test]
            public void Should_add_action_for_specified_call_to_call_actions()
            {
                _callActions.received(x => x.Add(_callSpec, _action));
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
                _call = mock<ICall>();
                _action = args => { };
                _callActions = mock<ICallActions>();
                _callSpec = mock<ICallSpecification>();
                _matchArgs = MatchArgs.AsSpecifiedInCall;

                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _matchArgs)).Return(_callSpec);
            }

            public override SetActionForCallHandler CreateSubjectUnderTest()
            {
                return new SetActionForCallHandler(_callSpecificationFactory, _callActions, _action, _matchArgs);
            }
        }
    }
}