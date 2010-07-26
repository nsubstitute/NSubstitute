using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.Examples
{
    public class SetReturnValues
    {
        //{CODE:set_return_values.ICalculator}
        public interface ICalculator
        {
            int Add(int a, int b);
            string Mode { get; set; }
        }

        [Test]
        public void Set_a_return_value_for_a_method()
        {
            //{CODE:set_return_values.for_a_method}
            var calculator = Substitute.For<ICalculator>();
            calculator.Add(1, 2).Returns(3); //Make this call return 3.
            Assert.AreEqual(calculator.Add(1, 2), 3);
            Assert.AreNotEqual(calculator.Add(3, 6), 3); 
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