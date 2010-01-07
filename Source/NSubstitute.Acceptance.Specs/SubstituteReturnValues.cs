using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstituteReturnValues
    {
        private ISomething _something;

        [Test]
        public void Return_a_single_value()
        {
            _something.Count().Return(3);

            Assert.That(_something.Count(), Is.EqualTo(3), "First return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Second return");
        }

        [Test]
        public void Return_multiple_values_from_different_calls()
        {
            _something.Echo(1).Return("one");
            _something.Echo(2).Return("two");

            Assert.That(_something.Echo(1), Is.EqualTo("one"), "First return");
            Assert.That(_something.Echo(2), Is.EqualTo("two"), "Second return");
        }

        [Test]
        [Pending]
        public void Return_multiple_values_from_the_same_call()
        {
            _something.Count().Return(1, 2, 3);

            Assert.That(_something.Count(), Is.EqualTo(1), "First return");
            Assert.That(_something.Count(), Is.EqualTo(2), "Second return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Third return");
            Assert.That(_something.Count(), Is.EqualTo(3), "Fourth return");
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}