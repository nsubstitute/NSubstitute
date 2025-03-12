using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue47_RaisingEventsWithNullArg
{
    public delegate void ProgressEventHandler(int progress, string message);
    public delegate void EventLikeHandler(object sender, EventArgs args);
    public interface IFoo
    {
        event ProgressEventHandler OnProgress;
        event EventLikeHandler OnEventishThing;
    }

    [Test]
    public void Pass_null_when_raising_delegate_event()
    {
        var sub = Substitute.For<IFoo>();
        sub.OnProgress += Raise.Event<ProgressEventHandler>(1, null);
    }

    [Test]
    public void Pass_null_when_raising_eventhandlerish_event()
    {
        var sub = Substitute.For<IFoo>();
        sub.OnEventishThing += Raise.Event<EventLikeHandler>([null]);
    }
}