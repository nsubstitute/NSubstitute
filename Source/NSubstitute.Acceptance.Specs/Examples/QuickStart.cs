using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.Examples
{
    public class QuickStart
    {
        private ICalculator _calculator;

        /* {CODE:basic_calculator_interface} */
        public interface ICalculator
        {
            int Add(int a, int b);
            string Mode { get; set; }
            event Action PoweringUp;
        }

        [SetUp]
        public void SetUp()
        {
            /* {CODE:substitute_for_calculator} */
            _calculator = Substitute.For<ICalculator>();
        }

        [Test]
        public void Return_a_value_for_a_call()
        {
            /* {CODE:return_a_value_for_a_call} */
            _calculator.Add(1, 2).Returns(3);
            Assert.That(_calculator.Add(1, 2), Is.EqualTo(3));
        }

        [Test]
        public void Check_substitute_received_some_calls_but_not_others()
        {
            /* {CODE:check_substitute_received_some_calls_but_not_others} */
            _calculator.Add(1, 2);
            _calculator.Received().Add(1, 2);
            _calculator.DidNotReceive().Add(5, 7);
        }

        [Test]
        public void Set_properties_with_returns_or_setters()
        {
            /* {CODE:set_properties_with_returns_or_setters} */
            _calculator.Mode.Returns("DEC");
            Assert.That(_calculator.Mode, Is.EqualTo("DEC"));

            _calculator.Mode = "HEX";
            Assert.That(_calculator.Mode, Is.EqualTo("HEX"));
        }

        [Test]
        public void Arg_matching()
        {
            /* {CODE:arg_matching} */
            _calculator.Add(10, -5);
            _calculator.Received().Add(Arg.Is(10), Arg.Any<int>());
            _calculator.Received().Add(Arg.Is(10), Arg.Is<int>(x => x < 0));
        }

        [Test]
        public void Arg_matching_and_behaviour()
        {
            /* {CODE:arg_matching_and_behaviour} */
            _calculator
               .Add(Arg.Any<int>(), Arg.Any<int>())
               .Returns(x => (int)x[0] + (int)x[1]);
            Assert.That(_calculator.Add(5, 10), Is.EqualTo(15));
        }

        [Test]
        public void Return_multiple_arguments()
        {
            /* {CODE:return_multiple_arguments} */
            _calculator.Mode.Returns("HEX", "DEC", "BIN");
            Assert.That(_calculator.Mode, Is.EqualTo("HEX"));
            Assert.That(_calculator.Mode, Is.EqualTo("DEC"));
            Assert.That(_calculator.Mode, Is.EqualTo("BIN"));
        }

        [Test]
        public void Raise_events()
        {
            /* {CODE:raise_events} */
            bool eventWasRaised = false;
            _calculator.PoweringUp += () => eventWasRaised = true;

            _calculator.PoweringUp += Raise.Action();
            Assert.That(eventWasRaised);
        }
    }
}