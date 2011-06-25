using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ArgumentInvocationFromMatchers
    {
        public interface IFoo
        {
            void DoSomething(string something, Action callback);
        }

        [Test]
        [Pending]
        public void Invoke_callback_from_matcher()
        {
            var wasCalled = false;
            Action action = () => wasCalled = true;
            var sub = Substitute.For<IFoo>();
            sub.DoSomething("test", Arg.Invoke<Action>());

            sub.DoSomething("test", action);

            Assert.That(wasCalled, "Action should have been invoked");
            sub.Received().DoSomething("test", action);
        }

        [Test]
        [Pending]
        public void Call_with_invoke_matcher_should_not_count_as_a_received_call()
        {
            var sub = Substitute.For<IFoo>();

            sub.DoSomething("test", Arg.Invoke<Action>());

            sub.DidNotReceiveWithAnyArgs().DoSomething(null, null);
        }
    }
}