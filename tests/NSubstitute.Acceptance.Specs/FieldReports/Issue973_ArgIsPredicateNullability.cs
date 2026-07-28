using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

[TestFixture]
public class Issue973_ArgIsPredicateNullability
{
    public interface ICalculator
    {
        int Add(string text);
    }

    [Test]
    public void ArgIs_WithNonNullablePredicate_DoesNotCauseNullabilityWarnings()
    {
        var calculator = Substitute.For<ICalculator>();

        calculator.Add(Arg.Is<string>(s => s.Length > 0)).Returns(10);

        var result = calculator.Add("hello");

        Assert.AreEqual(10, result);
    }
}
