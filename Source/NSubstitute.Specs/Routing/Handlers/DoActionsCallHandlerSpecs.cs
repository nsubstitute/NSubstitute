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
            private ICall _call;            
            private ICallActions _callActions;
            private RouteAction _result;

            [Test]
            public void Should_invoke_all_actions_for_call_with_call_information()
            {
                _callActions.received(x => x.InvokeMatchingActions(_call));
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
                _callActions = mock<ICallActions>();
            }

            public override DoActionsCallHandler CreateSubjectUnderTest()
            {
                return new DoActionsCallHandler(_callActions);
            }
        }
    }
}