using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SubstituteReceivedCalls
    {
        private IEngine _engine;
        private int _rpm;

        [Test]
        public void Pass_when_call_was_received()
        {
            _engine.Received().Rev();
        }

        [Test]
        public void Fail_when_call_was_not_received()
        {
            Assert.Throws<CallNotReceivedException>(() =>
                    _engine.Received().Idle()
                );
        }

        [Test]
        public void Pass_when_call_was_received_with_correct_argument()
        {
            _engine.Received().RevAt(_rpm);
        }

        [Test]
        public void Fail_when_call_was_received_with_different_argument()
        {
            Assert.Throws<CallNotReceivedException>(() =>
                    _engine.Received().RevAt(_rpm + 2)
                );
        }

        [SetUp]
        public void SetUp()
        {
            _rpm = 7000;
            _engine = Substitute.For<IEngine>();
            _engine.Rev();
            _engine.RevAt(_rpm);
        }

    }

}