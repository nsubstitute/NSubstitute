using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class RecordCallHandlerSpecs
    {
        public class When_handling_call_to_a_member : ConcernFor<RecordCallHandler>
        {
            ICall _call;
            ICallStack _callStack;
            private RouteAction _result;

            [Test]
            public void Should_record_call()
            {
                _callStack.received(x => x.Push(_call));
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
                _callStack = mock<ICallStack>();
                _call = mock<ICall>();
            }

            public override RecordCallHandler CreateSubjectUnderTest()
            {
                return new RecordCallHandler(_callStack);
            } 
        }
    }
}