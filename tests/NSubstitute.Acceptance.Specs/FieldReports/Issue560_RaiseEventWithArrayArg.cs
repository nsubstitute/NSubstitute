using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue560_RaiseEventWithArrayArg
{
    [Test]
    public void Raise_event_declared_as_action_with_ref_array_type()
    {
        Widget[] arg = null;
        Widget w1 = new Widget("hello");
        Widget w2 = new Widget("world");

        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionWithRefArrayType += x => arg = x;

        eventSamples.ActionWithRefArrayType += Raise.Event<Action<Widget[]>>(new[] { w1, w2 });
        Assert.That(arg, Is.Not.Null);
        Assert.That(arg.Length, Is.EqualTo(2));
        Assert.That(arg[0], Is.EqualTo(w1));
        Assert.That(arg[1], Is.EqualTo(w2));
    }

    [Test]
    public void Should_raise_event_for_object_array_arg()
    {
        object[] capturedArg = null;
        var arg1 = new[] { new object(), new object() };

        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionWithParamOfObjectArray += x => capturedArg = x;

        eventSamples.ActionWithParamOfObjectArray += Raise.Event<Action<object[]>>(arg1);
        Assert.That(capturedArg, Is.EqualTo(arg1));
    }

    [Test]
    public void Should_raise_event_for_object_array_arg_with_covariance()
    {
        object[] capturedArg = null;
        var arg1 = new[] { "hello", "world" };

        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionWithParamOfObjectArray += x => capturedArg = x;

        eventSamples.ActionWithParamOfObjectArray += Raise.Event<Action<object[]>>(arg1);
        Assert.That(capturedArg, Is.EqualTo(arg1));
    }

    [Test]
    public void Should_raise_event_for_object_array_arg_provided_without_using_params_syntax()
    {
        object[] capturedArg = null;
        var arg1 = new[] { new object(), new object() };

        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionWithParamOfObjectArray += x => capturedArg = x;

        eventSamples.ActionWithParamOfObjectArray += Raise.Event<Action<object[]>>([arg1]);
        Assert.That(capturedArg, Is.EqualTo(arg1));
    }

    [Test]
    public void Should_raise_event_for_object_array_arg_provided_without_using_params_syntax_with_covariance()
    {
        object[] capturedArg = null;
        var arg1 = new[] { "hello", "world" };

        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionWithParamOfObjectArray += x => capturedArg = x;

        eventSamples.ActionWithParamOfObjectArray += Raise.Event<Action<object[]>>([arg1]);
        Assert.That(capturedArg, Is.EqualTo(arg1));
    }

    [Test]
    public void Should_raise_event_for_multiple_object_array_args()
    {
        Tuple<object[], object[]> capturedArgs = null;
        var arg1 = new[] { new object(), new object() };
        var arg2 = new[] { new object(), new object() };

        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionWithParamsOfObjectArray += (a1, a2) => capturedArgs = Tuple.Create(a1, a2);

        eventSamples.ActionWithParamsOfObjectArray += Raise.Event<Action<object[], object[]>>(arg1, arg2);
        Assert.That(capturedArgs, Is.Not.Null);
        Assert.That(capturedArgs.Item1, Is.EqualTo(arg1));
        Assert.That(capturedArgs.Item2, Is.EqualTo(arg2));
    }

    public interface IEventSamples
    {
        event Action<Widget[]> ActionWithRefArrayType;
        event Action<object[]> ActionWithParamOfObjectArray;
        event Action<object[], object[]> ActionWithParamsOfObjectArray;
    }

    public class Widget(string name)
    {
        public string Name { get; } = name;
    }
}