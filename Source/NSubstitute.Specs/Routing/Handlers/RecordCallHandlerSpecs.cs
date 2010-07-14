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

            [Test]
            public void Should_record_call()
            {
                _callStack.received(x => x.Push(_call));
            }

            public override void Because()
            {
                sut.Handle(_call);
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