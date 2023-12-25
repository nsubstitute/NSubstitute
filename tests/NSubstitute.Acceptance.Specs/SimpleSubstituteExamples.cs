using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class SimpleSubstituteExamples
{
    public interface ICalculator
    {
        void SwitchOn();
        int Add(int a, int b);
    }

    [Test]
    public void Use_a_shiny_new_substitute()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.SwitchOn();
        Assert.That(calculator.Add(1, 2), Is.EqualTo(default(int)));
    }

    [Test]
    public void Tell_a_substitute_to_return_a_value()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.Add(1, 2).Returns(3);
        Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
    }

    [Test]
    public void Return_different_values_for_different_arguments()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.Add(1, 2).Returns(3);
        calculator.Add(20, 30).Returns(50);
        Assert.That(calculator.Add(20, 30), Is.EqualTo(50));
        Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
    }

    [Test]
    public void Return_a_value_evaluated_at_runtime()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => (int)x[0] + (int)x[1]);
        Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
        Assert.That(calculator.Add(5, 10), Is.EqualTo(15));
        Assert.That(calculator.Add(123, 456), Is.EqualTo(579));
        Assert.That(calculator.Add(-1, -2), Is.EqualTo(-3));
    }

    [Test]
    public void Check_a_call_was_received()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.Add(1, 2);
        calculator.Received().Add(1, 2);
    }

    [Test]
    public void Override_a_previously_set_value()
    {
        var calculator = Substitute.For<ICalculator>();
        calculator.Add(1, 2).Returns(3);
        Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
        calculator.Add(1, 2).Returns(3000);
        Assert.That(calculator.Add(1, 2), Is.EqualTo(3000));
    }
}