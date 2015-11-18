using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    /// <summary>
    /// Defines, a test class.
    /// <a href="https://github.com/nsubstitute/NSubstitute/issues/177">Issue #177 discussion.</a>
    /// </summary>
    public class Issue177_SubstituteOperationDefinitionWithOverlappingCases
    {
        #region Setup

        private ISomething _something;

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        } 

        #endregion

        #region Multiple values from calls with fixed args

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_fixed_args()
        {
            _something.Add(1, 1).Returns(1, 2, 3);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(1, 1)");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(1, 1)");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(1, 1)");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_fixed_args_last_value()
        {
            _something.Add(1, 1).Returns(1, 2, 3);

            for (int i = 0; i < 665; i++) _something.Add(1, 1);

            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "666 return from Add(1, 1)");
        }

        [Test]
        public void Return_multiple_value_results_from_last_add_call_with_fixed_args_that_overlap()
        {
            _something.Add(1, 1).Returns(1, 2, 3);
            _something.Add(1, 1).Returns(4, 5, 6);
            _something.Add(1, 1).Returns(7, 8, 9);

            Assert.That(_something.Add(1, 1), Is.EqualTo(7), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(8), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(9), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_fixed_args_then_another_add_call_single_return()
        {
            _something.Add(1, 1).Returns(1, 2, 3);
            _something.Add(2, 2).Returns(4);
            _something.Add(3, 3).Returns(5);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_fixed_args_then_another_add_call_with_fixed_args()
        {
            _something.Add(1, 1).Returns(1, 2, 3);
            _something.Add(2, 2).Returns(4, 5, 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(1, 1)");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(1, 1)");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(1, 1)");
            Assert.That(_something.Add(2, 2), Is.EqualTo(4), "First return from Add(2, 2)");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_fixed_args_then_other_call()
        {
            _something.Add(1, 1).Returns(1, 2, 3);
            _something.Count().Returns(4, 5, 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(1, 1)");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(1, 1)");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(1, 1)");
        }

        #endregion

        #region Multiple values from calls with any args complete

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_last_value()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);

            for (int i = 0; i < 665; i++) _something.Add(1, 1);

            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "666 return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_last_add_call_with_any_args_that_overlap()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(4, 5, 6);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(7, 8, 9);

            Assert.That(_something.Add(1, 1), Is.EqualTo(7), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(8), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(9), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_then_another_add_call_single_return()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Add(2, 2).Returns(4);
            _something.Add(3, 3).Returns(5);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_then_another_add_call_with_fixed_args()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Add(1, 1).Returns(4, 5, 6);

            Assert.That(_something.Add(1, 2), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(3, 4), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(5, 6), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(1, 1)");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_then_other_call()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Count().Returns(4, 5, 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_order1()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Add(1, 1).Returns(4, 5, 6);
            _something.Add(2, 2).Returns(7, 8, 9);
            _something.Add(3, 3).Returns(10, 11, 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_order2()
        {
            _something.Add(1, 1).Returns(4, 5, 6);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Add(2, 2).Returns(7, 8, 9);
            _something.Add(3, 3).Returns(10, 11, 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_order3()
        {
            _something.Add(1, 1).Returns(4, 5, 6);
            _something.Add(2, 2).Returns(7, 8, 9);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Add(3, 3).Returns(10, 11, 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_echo_call_any_args_then_repeat()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Echo(Arg.Any<int>()).Returns("Echo");
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(4, 5, 6);
            _something.Echo(Arg.Any<int>()).Returns("Echo");

            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_then_echo_call_any_args_then_other_call()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(1, 2, 3);
            _something.Echo(Arg.Any<int>()).Returns("Echo");
            _something.Count().Returns(4, 5, 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Echo(1), Is.EqualTo("Echo"), "First return from Echo(Arg.Any<int>())");
        }

        #endregion

        #region Multiple values from calls with any args partial

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(1, 2, 3);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_last_value()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(1, 2, 3);

            for (int i = 0; i < 665; i++) _something.Add(1, 1);

            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "666 return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_last_add_call_with_any_args_partial_that_overlap()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(1, 2, 3);
            _something.Add(Arg.Any<int>(), 1).Returns(4, 5, 6);
            _something.Add(Arg.Any<int>(), 1).Returns(7, 8, 9);

            Assert.That(_something.Add(1, 1), Is.EqualTo(7), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(8), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(9), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_then_another_add_call_single_return()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(1, 2, 3);
            _something.Add(2, 2).Returns(4);
            _something.Add(3, 3).Returns(5);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_then_another_add_call_with_fixed_args()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(1, 2, 3);
            _something.Add(1, 1).Returns(4, 5, 6);

            Assert.That(_something.Add(2, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(3, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(5, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(1, 1)");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_then_other_call()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(1, 2, 3);
            _something.Count().Returns(4, 5, 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_order1()
        {
            _something.Add(Arg.Any<int>(), 7).Returns(1, 2, 3);
            _something.Add(1, 1).Returns(4, 5, 6);
            _something.Add(2, 2).Returns(7, 8, 9);
            _something.Add(3, 3).Returns(10, 11, 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_order2()
        {
            _something.Add(1, 1).Returns(4, 5, 6);
            _something.Add(Arg.Any<int>(), 7).Returns(1, 2, 3);
            _something.Add(2, 2).Returns(7, 8, 9);
            _something.Add(3, 3).Returns(10, 11, 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_order3()
        {
            _something.Add(1, 1).Returns(4, 5, 6);
            _something.Add(2, 2).Returns(7, 8, 9);
            _something.Add(Arg.Any<int>(), 7).Returns(1, 2, 3);
            _something.Add(3, 3).Returns(10, 11, 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_echo_call_any_args_then_repeat()
        {
            _something.Add(Arg.Any<int>(), 7).Returns(1, 2, 3);
            _something.Echo(Arg.Any<int>()).Returns("Echo");
            _something.Add(Arg.Any<int>(), 7).Returns(4, 5, 6);
            _something.Echo(Arg.Any<int>()).Returns("Echo");

            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_value_results_from_the_add_call_with_any_args_partial_then_echo_call_any_args_then_other_call()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(1, 2, 3);
            _something.Echo(Arg.Any<int>()).Returns("Echo");
            _something.Count().Returns(4, 5, 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Echo(1), Is.EqualTo("Echo"), "First return from Echo(Arg.Any<int>())");
        }

        #endregion

        #region Multiple funcs from calls with any args complete

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_last_value()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);

            for (int i = 0; i < 665; i++) _something.Add(1, 1);

            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "666 return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_last_add_call_with_any_args_that_overlap()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 4, x => 5, x => 6);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 7, x => 8, x => 9);

            Assert.That(_something.Add(1, 1), Is.EqualTo(7), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(8), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(9), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_then_another_add_call_single_return()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Add(2, 2).Returns(x => 4);
            _something.Add(3, 3).Returns(x => 5);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_then_another_add_call_with_fixed_args()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);

            Assert.That(_something.Add(1, 2), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(3, 4), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(5, 6), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(1, 1)");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_then_other_call()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Count().Returns(x => 4, x => 5, x => 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_order1()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);
            _something.Add(2, 2).Returns(x => 7, x => 8, x => 9);
            _something.Add(3, 3).Returns(x => 10, x => 11, x => 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_order2()
        {
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Add(2, 2).Returns(x => 7, x => 8, x => 9);
            _something.Add(3, 3).Returns(x => 10, x => 11, x => 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_order3()
        {
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);
            _something.Add(2, 2).Returns(x => 7, x => 8, x => 9);
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Add(3, 3).Returns(x => 10, x => 11, x => 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_echo_call_any_args_then_repeat()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Echo(Arg.Any<int>()).Returns(x => "Echo");
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 4, x => 5, x => 6);
            _something.Echo(Arg.Any<int>()).Returns(x => "Echo");

            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_then_echo_call_any_args_then_other_call()
        {
            _something.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => 1, x => 2, x => 3);
            _something.Echo(Arg.Any<int>()).Returns(x => "Echo");
            _something.Count().Returns(x => 4, x => 5, x => 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Echo(1), Is.EqualTo("Echo"), "First return from Echo(Arg.Any<int>())");
        }

        #endregion

        #region Multiple funcs from calls with any args partial

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(x => 1, x => 2, x => 3);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_last_value()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(x => 1, x => 2, x => 3);

            for (int i = 0; i < 665; i++) _something.Add(1, 1);

            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "666 return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_last_add_call_with_any_args_partial_that_overlap()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(x => 1, x => 2, x => 3);
            _something.Add(Arg.Any<int>(), 1).Returns(x => 4, x => 5, x => 6);
            _something.Add(Arg.Any<int>(), 1).Returns(x => 7, x => 8, x => 9);

            Assert.That(_something.Add(1, 1), Is.EqualTo(7), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(8), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(9), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_then_another_add_call_single_return()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(x => 1, x => 2, x => 3);
            _something.Add(2, 2).Returns(x => 4);
            _something.Add(3, 3).Returns(x => 5);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_then_another_add_call_with_fixed_args()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(x => 1, x => 2, x => 3);
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);

            Assert.That(_something.Add(1, 2), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(3, 4), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(5, 6), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(1, 1)");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_then_other_call()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(x => 1, x => 2, x => 3);
            _something.Count().Returns(x => 4, x => 5, x => 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(2), "Second return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Add(1, 1), Is.EqualTo(3), "Third return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_order1()
        {
            _something.Add(Arg.Any<int>(), 7).Returns(x => 1, x => 2, x => 3);
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);
            _something.Add(2, 2).Returns(x => 7, x => 8, x => 9);
            _something.Add(3, 3).Returns(x => 10, x => 11, x => 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_order2()
        {
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);
            _something.Add(Arg.Any<int>(), 7).Returns(x => 1, x => 2, x => 3);
            _something.Add(2, 2).Returns(x => 7, x => 8, x => 9);
            _something.Add(3, 3).Returns(x => 10, x => 11, x => 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_order3()
        {
            _something.Add(1, 1).Returns(x => 4, x => 5, x => 6);
            _something.Add(2, 2).Returns(x => 7, x => 8, x => 9);
            _something.Add(Arg.Any<int>(), 7).Returns(x => 1, x => 2, x => 3);
            _something.Add(3, 3).Returns(x => 10, x => 11, x => 12);

            Assert.That(_something.Add(7, 7), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_echo_call_any_args_then_repeat()
        {
            _something.Add(Arg.Any<int>(), 7).Returns(x => 1, x => 2, x => 3);
            _something.Echo(Arg.Any<int>()).Returns(x => "Echo");
            _something.Add(Arg.Any<int>(), 7).Returns(x => 4, x => 5, x => 6);
            _something.Echo(Arg.Any<int>()).Returns(x => "Echo");

            Assert.That(_something.Add(1, 1), Is.EqualTo(4), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
        }

        [Test]
        public void Return_multiple_func_results_from_the_add_call_with_any_args_partial_then_echo_call_any_args_then_other_call()
        {
            _something.Add(Arg.Any<int>(), 1).Returns(x => 1, x => 2, x => 3);
            _something.Echo(Arg.Any<int>()).Returns(x => "Echo");
            _something.Count().Returns(x => 4, x => 5, x => 6);

            Assert.That(_something.Add(1, 1), Is.EqualTo(1), "First return from Add(Arg.Any<int>(), Arg.Any<int>())");
            Assert.That(_something.Echo(1), Is.EqualTo("Echo"), "First return from Echo(Arg.Any<int>())");
        }

        #endregion
    }
}
