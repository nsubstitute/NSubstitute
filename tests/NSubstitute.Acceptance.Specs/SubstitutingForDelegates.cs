using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class SubstitutingForDelegates
{
    [Test]
    public void SubForAction()
    {
        var action = Substitute.For<Action>();
        action();
        action.Received()();
    }

    [Test]
    public void SubForActionWith2Parameters()
    {
        var action = Substitute.For<Action<int, int>>();
        action(4, 2);
        action.Received()(4, 2);
    }

    [Test]
    public void SubForFunc()
    {
        var func = Substitute.For<Func<int, string>>();
        func(1).Returns("1");

        Assert.That(func(1), Is.EqualTo("1"));
        func.Received()(1);
    }

    [Test]
    public void SubForFuncThatReturnsValueType()
    {
        var func = Substitute.For<Func<int>>();
        func().Returns(10);
        ClassicAssert.AreEqual(10, func());
    }

    [Test]
    public void substitute_for_an_event_handler()
    {
        var eventHandler = Substitute.For<EventHandler>();
        eventHandler.Invoke(null, null);

        eventHandler.Received().Invoke(null, null);
    }

    [Test]
    public void substitute_for_an_eventhandler()
    {
        var eventHandler = Substitute.For<EventHandler>();
        eventHandler(null, null);

        eventHandler.Received()(null, null);
    }

}