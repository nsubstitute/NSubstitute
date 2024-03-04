using NUnit.Framework;
using NUnit.Framework.Legacy;

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

        ClassicAssert.Contains(ret1, results);
        ClassicAssert.Contains(ret2, results);
    }
}
