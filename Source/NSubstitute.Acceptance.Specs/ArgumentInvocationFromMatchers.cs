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
            int MethodWithCallbackWithArgumentsAndReturnValue(string something, Action<int, string> callback);
        }

        [Test]
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
        public void Call_with_invoke_matcher_should_not_count_as_a_received_call()
        {
            var sub = Substitute.For<IFoo>();

            sub.MethodWithCallback("test", Arg.Invoke());

            sub.DidNotReceiveWithAnyArgs().MethodWithCallback(null, null);
        }

        [Test]
        public void Invoke_callback_as_well_as_return_a_value_for_call()
        {
            const int expectedReturnValue = 42;
            var sub = Substitute.For<IFoo>();
            var action = Substitute.For<Action<int, string>>();
            sub.MethodWithCallbackWithArgumentsAndReturnValue("test", Arg.Invoke(1, "hello")).Returns(expectedReturnValue);

            var result = sub.MethodWithCallbackWithArgumentsAndReturnValue("test", action);

            action.Received().Invoke(1, "hello");
            Assert.That(result, Is.EqualTo(expectedReturnValue));
        }

        [Test]
        public void Set_return_for_any_args_but_do_not_invoke_callback_when_args_do_not_match()
        {
            const int expectedReturnValue = 42;
            var sub = Substitute.For<IFoo>();
            var action = Substitute.For<Action<int, string>>();
            sub
                .MethodWithCallbackWithArgumentsAndReturnValue("test", Arg.Invoke(1, "hello"))
                .ReturnsForAnyArgs(expectedReturnValue);

            var result = sub.MethodWithCallbackWithArgumentsAndReturnValue("different arg", action);

            action.DidNotReceive().Invoke(1, "hello");
            Assert.That(result, Is.EqualTo(expectedReturnValue));
        }

        [Test]
        public void Invoke_callback_and_set_return_for_any_arguments()
        {
            const int expectedReturnValue = 42;
            var sub = Substitute.For<IFoo>();
            var action = Substitute.For<Action<int, string>>();
            sub
                .MethodWithCallbackWithArgumentsAndReturnValue("test", Arg.Invoke(1, "hello"))
                .ReturnsForAnyArgs(expectedReturnValue);

            var result = sub.MethodWithCallbackWithArgumentsAndReturnValue("test", action);

            action.Received().Invoke(1, "hello");
            Assert.That(result, Is.EqualTo(expectedReturnValue));
        }

        [Test]
        public void Invoke_callback_using_when_do()
        {
            var sub = Substitute.For<IFoo>();
            var action = Substitute.For<Action<int, string>>();
            sub.When(x => x.MethodWithCallbackWithArguments("test", Arg.Invoke(1, "hello"))).Do(x => { });

            sub.MethodWithCallbackWithArguments("test", action);

            action.Received().Invoke(1, "hello");
        }

        [Test]
        [Ignore("What should this do?")]
        public void Invoke_callback_using_when_for_any_args_do()
        {
            var sub = Substitute.For<IFoo>();
            var action = Substitute.For<Action<int, string>>();
            sub.WhenForAnyArgs(x => x.MethodWithCallbackWithArguments("test", Arg.Invoke(1, "hello"))).Do(x => { });

            sub.MethodWithCallbackWithArguments("something else", action);
            action.DidNotReceive().Invoke(1, "hello");

            sub.MethodWithCallbackWithArguments("test", action);
            action.Received().Invoke(1, "hello");
        }
    }
}