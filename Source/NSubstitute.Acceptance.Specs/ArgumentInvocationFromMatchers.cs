using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ArgumentInvocationFromMatchers
    {
        public delegate void ActionCompatibleDelegate(int i);
        public interface IFoo
        {
            void MethodWithCallback(string something, Action callback);
            void MethodWithCallbackWithArguments(string something, Action<int, string> callback);
            void MethodWithDelegateCallback(string something, ActionCompatibleDelegate callback);
        }

        [Test]
        [Pending]
        public void Invoke_callback_from_matcher()
        {
            var action = Substitute.For<Action>();
            var sub = Substitute.For<IFoo>();
            sub.MethodWithCallback("test", Arg.Invoke());

            sub.MethodWithCallback("test", action);

            action.Received().Invoke();
            sub.Received().MethodWithCallback("test", action);
        }

        [Test]
        [Pending]
        public void Invoke_callback_with_arguments()
        {
            var action = Substitute.For<Action<int, string>>();
            var sub = Substitute.For<IFoo>();
            sub.MethodWithCallbackWithArguments("test", Arg.Invoke(1, "hello"));

            sub.MethodWithCallbackWithArguments("test", action);

            action.Received().Invoke(1, "hello");
            sub.Received().MethodWithCallbackWithArguments("test", action);
        }

        [Test]
        [Pending]
        public void Invoke_callback_with_argument_using_specific_delegate_type()
        {
            var action = Substitute.For<Action<int, string>>();
            var sub = Substitute.For<IFoo>();
            sub.MethodWithCallbackWithArguments("test", Arg.InvokeDelegate<Action<int, string>>(1, "hello"));

            sub.MethodWithCallbackWithArguments("test", action);

            action.Received().Invoke(1, "hello");
            sub.Received().MethodWithCallbackWithArguments("test", action);
        }

        [Test]
        [Pending]
        public void Invoke_delegate_callback()
        {
            var action = Substitute.For<Action<int>>();
            ActionCompatibleDelegate @delegate = x => action(x);
            var sub = Substitute.For<IFoo>();
            sub.MethodWithDelegateCallback("test", Arg.InvokeDelegate<ActionCompatibleDelegate>(1));

            sub.MethodWithDelegateCallback("test", @delegate);

            action.Received().Invoke(1);
            sub.Received().MethodWithDelegateCallback("test", @delegate);
        }

        [Test]
        [Pending]
        public void Call_with_invoke_matcher_should_not_count_as_a_received_call()
        {
            var sub = Substitute.For<IFoo>();

            sub.MethodWithCallback("test", Arg.Invoke());

            sub.DidNotReceiveWithAnyArgs().MethodWithCallback(null, null);
        }
    }
}