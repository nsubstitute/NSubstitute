using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstituteParameterMatching
    {
        private ISomething _something;

        [Test]
        public void Return_result_for_any_parameter()
        {
            _something.Echo(Arg.Any<int>()).Return("anything");

            Assert.That(_something.Echo(1), Is.EqualTo("anything"), "First return");
            Assert.That(_something.Echo(2), Is.EqualTo("anything"), "Second return");
        }

        [Test]
        public void Return_result_for_specific_parameter()
        {
            _something.Echo(Arg.Is(3)).Return("three");
            _something.Echo(4).Return("four");

            Assert.That(_something.Echo(3), Is.EqualTo("three"), "First return");
            Assert.That(_something.Echo(4), Is.EqualTo("four"), "Second return");
        }

        [Test]
        public void Return_result_for_parameter_matching_predicate()
        {
            _something.Echo(Arg.Is<int>(x => x <= 3)).Return("small");
            _something.Echo(Arg.Is<int>(x => x > 3)).Return("big");

            Assert.That(_something.Echo(1), Is.EqualTo("small"), "First return");
            Assert.That(_something.Echo(4), Is.EqualTo("big"), "Second return");
        }

        [Test]
        [Pending]
        public void Received_for_any_parameter()
        {
            _something.Echo(7);

            _something.Received().Echo(Arg.Any<int>());
        }

        [Test]
        [Pending]
        public void Recieved_for_specific_parameter()
        {
            _something.Echo(3);
            
            _something.Received().Echo(Arg.Is(3));
        }

        [Test]
        [Pending]
        public void Recieved_for_parameter_matching_predicate()
        {
            _something.Echo(7);

            _something.Received().Echo(Arg.Is<int>(x => x > 3));
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}