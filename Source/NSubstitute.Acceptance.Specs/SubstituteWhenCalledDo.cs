using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstituteWhenCalledDo
    {
        private ISomething _something;

        [Test]
        [Pending]
        public void Execute_when_called()
        {
            var called = false;
            _something.When(substitute => substitute.Echo(1)).Do(args => called = true);

            Assert.That(called, Is.False, "Called");
            _something.Echo(1);
            Assert.That(called, Is.True, "Called");
        }

        [Test]
        [Pending]
        public void Capture_arguments_when_called()
        {
            int firstArgument = 0;
            _something.When(substitute => substitute.Echo(1)).Do(args => firstArgument = (int)args[0]);

            Assert.That(firstArgument, Is.EqualTo(0), "firstArgument");
            _something.Echo(1);
            Assert.That(firstArgument, Is.EqualTo(1), "firstArgument");
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}