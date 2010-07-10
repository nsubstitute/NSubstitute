using System;
using NSubstitute.Core;
using NSubstitute.Routes.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes.Handlers
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
            private bool _forAnyArguments;

            [Test]
            public void Should_add_action_for_specified_call_to_call_actions()
            {
                _callActions.received(x => x.Add(_callSpec, _action));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _action = args => { };
                _callActions = mock<ICallActions>();
                _callSpec = mock<ICallSpecification>();
                _forAnyArguments = true;

                _callSpecificationFactory = mock<ICallSpecificationFactory>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _forAnyArguments)).Return(_callSpec);
            }

            public override SetActionForCallHandler CreateSubjectUnderTest()
            {
                return new SetActionForCallHandler(_callSpecificationFactory, _callActions, _action, _forAnyArguments);
            }
        }
    }
}