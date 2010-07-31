using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.Examples
{
    public class SetReturnValues
    {
        private ICalculator _calculator;

        //{CODE:set_return_values.ICalculator}
        public interface ICalculator
        {
            int Add(int a, int b);
            string Mode { get; set; }
        }

        [SetUp]
        public void Set_a_return_value_for_a_method()
        {
            //{CODE:set_return_values.for_a_method}
            _calculator = Substitute.For<ICalculator>();
            _calculator.Add(1, 2).Returns(3); //Make this call return 3.
        }

        [Test]
        public void Method_returns_that_value_for_matching_calls()
        {
            //{CODE:set_return_values.matching_calls}
            Assert.AreEqual(_calculator.Add(1, 2), 3);
            Assert.AreEqual(_calculator.Add(1, 2), 3);
            //Call with different arguments does not retun 3
            Assert.AreNotEqual(_calculator.Add(3, 6), 3); 
        }

        [Test]
        public void Return_value_does_not_apply_unless_arguments_match()
        {
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3); //Make this call return 3.
            //{CODE:set_return_values.non_matching_args}
            Assert.AreNotEqual(calculator.Add(3, 6), 3); 
        }

        [Test]
        public void Set_a_return_value_for_any_combination_of_arguments()
        {
            
        }
    }
}