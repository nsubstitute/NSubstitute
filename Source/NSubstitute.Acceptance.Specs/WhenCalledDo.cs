using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class WhenCalledDo
    {
        private ISomething _something;

        [Test]
        public void Execute_when_called()
        {
            var called = false;
            _something.When(substitute => substitute.Echo(1)).Do(info => called = true);

            Assert.That(called, Is.False, "Called");
            _something.Echo(1);
            Assert.That(called, Is.True, "Called");
        }

        [Test]
        public void Capture_arguments_when_called()
        {
            int firstArgument = 0;
            _something.When(substitute => substitute.Echo(1)).Do(info => firstArgument = (int)info[0]);

            Assert.That(firstArgument, Is.EqualTo(0), "firstArgument");
            _something.Echo(1);
            Assert.That(firstArgument, Is.EqualTo(1), "firstArgument");
        }

        [Test]
        public void Run_multiple_actions_when_called()
        {
            int called = 0;
            _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
            _something.When(x => x.Echo(4)).Do(x => called++);
            _something.WhenForAnyArgs(x => x.Echo(1234)).Do(x => called++);

            Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
            _something.Echo(4);
            Assert.That(called, Is.EqualTo(3));
        }

        [Test]
        public void Only_do_matching_actions()
        {
            int called = 0;
            _something.When(x => x.Echo(Arg.Any<int>())).Do(x => called++);
            _something.When(x => x.Echo(4)).Do(x => called++);

            Assert.That(called, Is.EqualTo(0), "Should not have been called yet");
            _something.Echo(1);
            Assert.That(called, Is.EqualTo(1));
        }

        [Test]
        public void Execute_when_called_for_any_args()
        {
            var called = false;
            _something.WhenForAnyArgs(x => x.Echo(1)).Do(x => called = true);

            Assert.That(called, Is.False, "Called");
            _something.Echo(1234);
            Assert.That(called, Is.True, "Called");
        }

        [SetUp]
        public void SetUp()
        {
            _something = Substitute.For<ISomething>();
        }
    }
}