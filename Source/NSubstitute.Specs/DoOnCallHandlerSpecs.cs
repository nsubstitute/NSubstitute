using System;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class DoOnCallHandlerSpecs
    {
        public class When_handling_a_call : ConcernFor<DoOnCallHandler>
        {
            private Action<object[]> _actionToPerform;
            private ICall _call;
            private object[] _callArguments;

            [Test]
            public void Should_perform_requested_action_with_call_arguments()
            {
                _actionToPerform.received(x => x(_callArguments));
            }

            public override void Because()
            {
                sut.Handle(_call);
            }

            public override void Context()
            {
                _actionToPerform = mock<Action<object[]>>();
                _call = mock<ICall>();
                _callArguments = new[] {new object(), new object()};
                _call.stub(x => x.GetArguments()).Return(_callArguments);
            }

            public override DoOnCallHandler CreateSubjectUnderTest()
            {
                return new DoOnCallHandler(_actionToPerform);
            }
        }
        
    }
}