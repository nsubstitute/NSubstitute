using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue_RaiseEventOnNonSubstitute
{
    public class Button
    {
        public virtual event EventHandler Clicked = (s, e) => { };
    }

    public interface IController { void Load(); }

    [Test]
    public void RaiseEventOnNonSub()
    {
        var notASub = new Button();
        notASub.Clicked += Raise.Event();
        var sub = Substitute.For<IController>();
        // Next call to a substitute will fail as it will attempt to raise an event
        Assert.Throws<CouldNotRaiseEventException>(() =>
            sub.Load()
        );
    }

    [Test]
    public void RaiseEventOnSub()
    {
        var clicked = false;
        var sub = Substitute.For<Button>();
        sub.Clicked += (s, e) => clicked = true;
        sub.Clicked += Raise.Event();
        Assert.That(clicked);
    }
}