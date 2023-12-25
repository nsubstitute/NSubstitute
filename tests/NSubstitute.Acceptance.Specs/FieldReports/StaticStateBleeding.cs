using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class StaticStateBleeding
{
    [Test]
    public void Test_0()
    {
        var bar = Substitute.For<IBar>();
        bar.GetInt();
        bar.Received().GetInt();
    }

    [Test]
    public void Test_1_affected_by_test_0()
    {
        Assert.Throws<CouldNotSetReturnDueToNoLastCallException>(() => 2.Returns(2));
    }

    public interface IFoo { }

    public interface IBar { int GetInt(); }
}