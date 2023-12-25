using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class ArgumentInvocationFromMatchers
{
    public delegate void ActionCompatibleDelegate(int i);
    public interface IFoo
    {
        void MethodWithCallback(string something, Action callback);
        void MethodWithCallbackWithArguments(string something, Action<int> callback);
        void MethodWithCallbackWithArguments(string something, Action<int, string> callback);
        void MethodWithCallbackWithArguments(string something, Action<int, string, double> callback);
        void MethodWithCallbackWithArguments(string something, Action<int, string, double, char> callback);
        void MethodWithDelegateCallback(string something, ActionCompatibleDelegate callback);
        int MethodWithCallbackWithArgumentsAndReturnValue(string something, Action<int, string> callback);
        void MethodWithRefCallback(string something, ref Action callback);
        void MethodWithRefCallbackWithArguments(string something, ref Action<int> callback);
        void MethodWithRefDelegateCallback(string something, ref ActionCompatibleDelegate callback);
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
    public void Invoke_ref_callback_from_matcher()
    {
        var action = Substitute.For<Action>();
        var sub = Substitute.For<IFoo>();
        sub.MethodWithRefCallback("test", ref Arg.Invoke());

        sub.MethodWithRefCallback("test", ref action);

        action.Received().Invoke();
        sub.Received().MethodWithRefCallback("test", ref action);
    }

    [Test]
    public void Invoke_callback_with_arguments()
    {
        var sub = Substitute.For<IFoo>();

        var action1 = Substitute.For<Action<int>>();
        sub.MethodWithCallbackWithArguments("test", Arg.Invoke(1));
        sub.MethodWithCallbackWithArguments("test", action1);
        action1.Received().Invoke(1);
        sub.Received().MethodWithCallbackWithArguments("test", action1);
    }

    [Test]
    public void Invoke_ref_callback_with_arguments()
    {
        var sub = Substitute.For<IFoo>();

        var action1 = Substitute.For<Action<int>>();
        sub.MethodWithRefCallbackWithArguments("test", ref Arg.Invoke(1));
        sub.MethodWithRefCallbackWithArguments("test", ref action1);
        action1.Received().Invoke(1);
        sub.Received().MethodWithRefCallbackWithArguments("test", ref action1);
    }
    [Test]
    public void Invoke_callback_with_two_arguments()
    {
        var sub = Substitute.For<IFoo>();
        var action2 = Substitute.For<Action<int, string>>();
        sub.MethodWithCallbackWithArguments("test", Arg.Invoke(1, "hello"));
        sub.MethodWithCallbackWithArguments("test", action2);
        action2.Received().Invoke(1, "hello");
        sub.Received().MethodWithCallbackWithArguments("test", action2);
    }

    [Test]
    public void Invoke_callback_with_three_arguments()
    {
        var sub = Substitute.For<IFoo>();
        var action3 = Substitute.For<Action<int, string, double>>();
        sub.MethodWithCallbackWithArguments("test", Arg.Invoke(1, "hello", 3.14));
        sub.MethodWithCallbackWithArguments("test", action3);
        action3.Received().Invoke(1, "hello", 3.14);
        sub.Received().MethodWithCallbackWithArguments("test", action3);
    }

    [Test]
    public void Invoke_callback_with_four_arguments()
    {
        var sub = Substitute.For<IFoo>();
        var action4 = Substitute.For<Action<int, string, double, char>>();
        sub.MethodWithCallbackWithArguments("test", Arg.Invoke(1, "hello", 3.14, '!'));
        sub.MethodWithCallbackWithArguments("test", action4);
        action4.Received().Invoke(1, "hello", 3.14, '!');
        sub.Received().MethodWithCallbackWithArguments("test", action4);
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
    public void Invoke_ref_delegate_callback()
    {
        var action = Substitute.For<Action<int>>();
        ActionCompatibleDelegate @delegate = x => action(x);
        var sub = Substitute.For<IFoo>();
        sub.MethodWithRefDelegateCallback("test", ref Arg.InvokeDelegate<ActionCompatibleDelegate>(1));

        sub.MethodWithRefDelegateCallback("test", ref @delegate);

        action.Received().Invoke(1);
        sub.Received().MethodWithRefDelegateCallback("test", ref @delegate);
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
    public void Set_return_for_any_args_should_invoke_callback_when_args_do_not_match()
    {
        const int expectedReturnValue = 42;
        var sub = Substitute.For<IFoo>();
        var action = Substitute.For<Action<int, string>>();
        sub
            .MethodWithCallbackWithArgumentsAndReturnValue("test", Arg.Invoke(1, "hello"))
            .ReturnsForAnyArgs(expectedReturnValue);

        var result = sub.MethodWithCallbackWithArgumentsAndReturnValue("different arg", action);

        action.Received().Invoke(1, "hello");
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
    public void Invoke_callback_using_when_for_any_args_do()
    {
        var sub = Substitute.For<IFoo>();
        var action = Substitute.For<Action<int, string>>();
        sub.WhenForAnyArgs(x => x.MethodWithCallbackWithArguments(null, Arg.Invoke(1, "hello"))).Do(x => { });

        sub.MethodWithCallbackWithArguments("something else", action);
        action.Received().Invoke(1, "hello");
    }

    [Test]
    public void Invoke_multiple_callbacks()
    {
        var action = Substitute.For<Action<int, string>>();
        var sub = Substitute.For<IFoo>();
        sub.MethodWithCallbackWithArguments("test", Arg.Invoke(1, "hello"));
        sub.MethodWithCallbackWithArguments("test", Arg.Invoke(2, "bye"));

        sub.MethodWithCallbackWithArguments("test", action);

        action.Received().Invoke(1, "hello");
        action.Received().Invoke(2, "bye");
        Assert.That(action.ReceivedCalls().Count(), Is.EqualTo(2));
        sub.Received().MethodWithCallbackWithArguments("test", action);
        Assert.That(sub.ReceivedCalls().Count(), Is.EqualTo(1));
    }

    [Test]
    public void Insanity_test()
    {
        var result = -1;
        var sub = Substitute.For<IFoo>();
        var actions = new[] { Substitute.For<Action<int, string>>(), Substitute.For<Action<int, string>>(), Substitute.For<Action<int, string>>() };

        sub.WhenForAnyArgs(x => x.MethodWithCallbackWithArgumentsAndReturnValue(null, Arg.Invoke(1, "que?"))).Do(x => { });
        sub.MethodWithCallbackWithArgumentsAndReturnValue("hello", Arg.Invoke(2, "hello")).ReturnsForAnyArgs(2);
        sub.MethodWithCallbackWithArgumentsAndReturnValue("bye", Arg.Invoke(3, "bye")).Returns(3);
        sub.MethodWithCallbackWithArgumentsAndReturnValue("hmm", Arg.Any<Action<int, string>>()).Returns(4);

        result = sub.MethodWithCallbackWithArgumentsAndReturnValue("something else", actions[0]);
        Assert.That(result, Is.EqualTo(2));
        actions[0].Received().Invoke(1, "que?");
        actions[0].Received().Invoke(2, "hello");
        Assert.That(actions[0].ReceivedCalls().Count(), Is.EqualTo(2));
        ClearAllCalls(actions);

        result = sub.MethodWithCallbackWithArgumentsAndReturnValue("bye", actions[0]);
        Assert.That(result, Is.EqualTo(3));
        actions[0].Received().Invoke(1, "que?");
        actions[0].Received().Invoke(2, "hello");
        actions[0].Received().Invoke(3, "bye");
        ClearAllCalls(actions);
    }

    private void ClearAllCalls(IEnumerable<object> subs)
    {
        foreach (var sub in subs) { sub.ClearReceivedCalls(); }
    }
}