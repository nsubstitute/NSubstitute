using NUnit.Framework;

namespace NSubstitute.Specs.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class SimpleSubstituteExamples
    {
        [Test]
        public void Use_a_shiny_new_substitute()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.SwitchOn();
            Assert.That(calculator.Add(1,2), Is.EqualTo(default(int)));
        }

        [Test]
        public void Tell_a_substitute_to_return_a_value()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Return(3);
            Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
        }
    }

    public interface ICalculator
    {
        void SwitchOn();
        int Add(int a, int b);
        int Subtract(int a, int b);
    }
}