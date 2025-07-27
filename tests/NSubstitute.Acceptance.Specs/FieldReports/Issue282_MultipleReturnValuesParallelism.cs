using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

[TestFixture]
public class Issue282_MultipleReturnValuesParallelism
{
    public interface IFoo
    {
        string Foo();
    }

    [Test]
    public void ReturnsMultipleValuesInParallel()
    {
        var ret1 = "One";
        var ret2 = "Two";

        var substitute = Substitute.For<IFoo>();
        substitute.Foo().Returns(ret1, ret2);

        var runningTask1 = Task.Run(() => substitute.Foo());
        var runningTask2 = Task.Run(() => substitute.Foo());

        var results = Task.WhenAll(runningTask1, runningTask2).Result;

        Assert.That(results, Does.Contain(ret1));
        Assert.That(results, Does.Contain(ret2));
    }
}
