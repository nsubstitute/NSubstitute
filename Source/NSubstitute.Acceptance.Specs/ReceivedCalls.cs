using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ReceivedCalls
    {
        private IEngine _engine;
        private int _rpm;

        [Test]
        public void Check_when_call_was_received()
        {
            _engine.Received().Rev();
        }

        [Test]
        public void Throw_when_expected_call_was_not_received()
        {
            Assert.Throws<CallNotReceivedException>(() =>
                    _engine.Received().Idle()
                );
        }

        [Test]
        public void Check_call_was_received_with_expected_argument()
        {
            _engine.Received().RevAt(_rpm);
        }

        [Test]
        public void Throw_when_expected_call_was_received_with_different_argument()
        {
            Assert.Throws<CallNotReceivedException>(() =>
                    _engine.Received().RevAt(_rpm + 2)
                );
        }

        [Test]
        public void Check_that_a_call_was_not_received()
        {
            _engine.DidNotReceive().RevAt(_rpm + 2); 
        }

        [Test][Pending]
        public void Throw_when_a_call_was_not_expected_to_be_received()
        {
            Assert.Throws<CallReceivedException>(() =>
                _engine.DidNotReceive().RevAt(_rpm)
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