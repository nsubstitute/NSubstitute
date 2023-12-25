using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue170_MultidimensionalArray
{
    public interface ITest
    {
        bool[,] Method();
    }

    [Test]
    public void Method_Works()
    {
        var test = Substitute.For<ITest>();
        test.Method();
    }
}