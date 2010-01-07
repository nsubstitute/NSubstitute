using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SequenceChecking
    {
        private IEngine _engine;
        private IIgnition _ignition;

        [Test]
        [Pending]
        public void ShouldCheckSequenceWithOneSubstitute()
        {
            _engine.Start();
            _engine.Rev();
            _engine.Stop();

            Received.This(() => _engine.Start())
                .Then(() => _engine.Rev())
                .Then(() => _engine.Stop());
        }

        [Test]
        [Pending]
        public void ShouldCheckSequenceWithMultipleSubstitutes()
        {
            _ignition.TurnRight();
            _engine.Start();
            _engine.Rev();
            _ignition.TurnLeft();
            _engine.Stop();

            Received.This(() => _ignition.TurnRight())
                .Then(() => _engine.Start())
                .Then(() => _engine.Rev())
                .Then(() => _ignition.TurnLeft())
                .Then(() => _engine.Stop());
        }

        [SetUp]
        public void SetUp()
        {
            _engine = Substitute.For<IEngine>();
            _ignition = Substitute.For<IIgnition>();
        }
    }

    public static class Received
    {
        public static ReceivedThis This(Action action)
        {
            return new ReceivedThis();
        }
    }

    public class ReceivedThis
    {
        public ReceivedThen Then(Action action)
        {
            return new ReceivedThen();
        }
    }

    public class ReceivedThen
    {
        public ReceivedThen Then(Action action)
        {
            return new ReceivedThen();
        }
    }
}