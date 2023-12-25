using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class SubbingForEventHandler
{
    [Test]
    public void Should_not_die_when_trying_to_sub_for_an_event_handler()
    {
        var sut = new Something();
        var handler = Substitute.For<EventHandler<SomethingEventArgs>>();
        sut.SomethingHappened += handler;
        sut.DoSomething();
        handler.Received().Invoke(sut, Arg.Is<SomethingEventArgs>(e => e.Somethings == 2));
    }

    public class Something
    {
        public void DoSomething()
        {
            OnSomething(2);
        }

        public event EventHandler<SomethingEventArgs> SomethingHappened;

        public void OnSomething(int somethings)
        {
            var handler = SomethingHappened;
            if (handler != null) handler(this, new SomethingEventArgs() { Somethings = somethings });
        }
    }

    public class SomethingEventArgs : EventArgs
    {
        public int Somethings;
    }
}