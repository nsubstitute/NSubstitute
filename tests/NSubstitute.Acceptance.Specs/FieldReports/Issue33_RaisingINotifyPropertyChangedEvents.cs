using System.ComponentModel;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue33_RaisingINotifyPropertyChangedEvents
{
    [Test]
    public void Should_be_able_to_raise_event()
    {
        var sub = Substitute.For<INotifyPropertyChanged>();
        bool wasCalled = false;
        sub.PropertyChanged += (sender, args) => wasCalled = true;

        sub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs("test"));

        Assert.That(wasCalled);
    }
}