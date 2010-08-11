using System.Linq;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ReturningResults
    {
        private ISomething _something;

        [Test]
        public void Return_a_single_result()
        {
            _something.Count().Returns(3);

            Assert.That(_something.Count(), Is.EqualTo(3), "First return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Second return");
        }

        [Test]
        public void Return_multiple_results_from_different_calls()
        {
            _something.Echo(1).Returns("one");
            _something.Echo(2).Returns("two");

            Assert.That(_something.Echo(1), Is.EqualTo("one"), "First return");
            Assert.That(_something.Echo(2), Is.EqualTo("two"), "Second return");
        }

        [Test]
        public void Return_multiple_results_from_the_same_call()
        {
            _something.Count().Returns(1, 2, 3);

            Assert.That(_something.Count(), Is.EqualTo(1), "First return");
            Assert.That(_something.Count(), Is.EqualTo(2), "Second return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Third return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Fourth return");
        }

        [Test]
        public void Return_result_for_any_arguments()
        {
            _something.Echo(1).ReturnsForAnyArgs("always");

            Assert.That(_something.Echo(1), Is.EqualTo("always"));
            Assert.That(_something.Echo(2), Is.EqualTo("always"));
            Assert.That(_something.Echo(724), Is.EqualTo("always"));
        }

        [Test]
        public void Return_multiple_results_for_any_arguments()
        {
            _something.Echo(1).ReturnsForAnyArgs("first", "second");

            Assert.That(_something.Echo(2), Is.EqualTo("first"));
            Assert.That(_something.Echo(724), Is.EqualTo("second"));
        }

        [Test]
        public void Return_calculated_results_for_any_arguments()
        {
            _something.Echo(-2).ReturnsForAnyArgs(x => x[0].ToString());

            Assert.That(_something.Echo(12), Is.EqualTo(12.ToString()));
            Assert.That(_something.Echo(123), Is.EqualTo(123.ToString()));
        }

        [Test]
        [Pending]
        public void Return_specific_value_for_tostring()
        {
            _something.ToString().Returns("this string");
            Assert.That(_something.ToString(), Is.EqualTo("this string"));
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}