using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Regression_ReceivedClearsStub
{
    public interface IFoo
    {
        IList<string> GetTheStrings();
    }

    [Test]
    public void MockShouldReturnTheStringListAfterCheckingCallHistory()
    {
        IFoo foo = Substitute.For<IFoo>();
        foo.GetTheStrings().Returns(new[] { "a", "b" });

        foo.DidNotReceive().GetTheStrings();
        var strings = foo.GetTheStrings();

        Assert.That(strings, Is.EquivalentTo(new[] { "a", "b" }));
    }
}