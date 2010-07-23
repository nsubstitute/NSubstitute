using System;
using NSubstitute.Acceptance.Specs.Infrastructure;
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
        public void Pass_when_checking_a_single_call_that_was_in_the_sequence()
        {
            _engine.Start();
            Received.This(() => _engine.Start());
        }

        [Test]
        [Pending]
        public void Pass_when_checking_a_subset_of_the_sequence_with_one_substitute()
        {
            Received.This(() => _engine.Start())
                .Then(() => _engine.Rev())
                .Then(() => _engine.Stop());
        }

        [Test]
        [Pending]
        public void Pass_when_checking_sequence_with_multiple_substitutes()
        {

            Received.This(() => _ignition.TurnRight())
                .Then(() => _engine.Start())
                .Then(() => _engine.Rev())
                .Then(() => _ignition.TurnLeft())
                .Then(() => _engine.Stop());
        }

        [Test]
        [Pending]
        public void Fail_when_one_of_the_calls_in_the_sequence_was_not_received()
        {
            Assert.Throws<CallSequenceNotFound>(() =>
                Received.This(() => _ignition.TurnRight())
                    .Then(() => _engine.Start())
                    .Then(() => _engine.Rev())
                    .Then(() => _engine.Idle())
                    .Then(() => _ignition.TurnLeft())                
                    .Then(() => _engine.Stop())
                );
        }

        [Test]
        [Pending]
        public void Fail_when_calls_made_in_different_order()
        {
            Assert.Throws<CallSequenceNotFound>(() =>
                Received.This(() => _ignition.TurnRight())
                    .Then(() => _engine.Stop())
                    .Then(() => _engine.Start())
                );
        }

        [SetUp]
        public void SetUp()
        {
            _engine = Substitute.For<IEngine>();
            _ignition = Substitute.For<IIgnition>();

            _ignition.TurnRight();
            _engine.Start();
            _engine.Rev();
            _ignition.TurnLeft();
            _engine.Stop();
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

    public class CallSequenceNotFound : Exception
    {        
    }
}