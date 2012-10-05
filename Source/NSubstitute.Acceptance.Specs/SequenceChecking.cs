using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Core;
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
            Received.CallTo(_engine, x => x.Start());
        }

        [Test]
        [Pending]
        public void Pass_when_checking_a_subset_of_the_sequence_with_one_substitute()
        {
            Received.CallTo(_engine, x => x.Start())
                .Then(_engine, x => x.Rev())
                .Then(_engine, x => x.Stop());
        }

        [Test]
        [Pending]
        public void Pass_when_checking_sequence_with_multiple_substitutes()
        {

            Received.CallTo(_ignition, x => x.TurnRight())
                .Then(_engine, x => x.Start())
                .Then(_engine, x => x.Rev())
                .Then(_ignition, x => x.TurnLeft())
                .Then(_engine, x => x.Stop());
        }

        [Test]
        [Pending]
        public void Fail_when_one_of_the_calls_in_the_sequence_was_not_received()
        {
            Assert.Throws<CallSequenceNotFound>(() =>
                Received.CallTo(_ignition, x => x.TurnRight())
                    .Then(_engine, x => x.Start())
                    .Then(_engine, x => x.Rev())
                    .Then(_engine, x => x.Idle())
                    .Then(_ignition, x => x.TurnLeft())
                    .Then(_engine, x => x.Stop())
                );
        }

        [Test]
        [Pending]
        public void Fail_when_calls_made_in_different_order()
        {
            Assert.Throws<CallSequenceNotFound>(() =>
                Received.CallTo(_ignition, x => x.TurnRight())
                    .Then(_engine, x => x.Stop())
                    .Then(_engine, x => x.Start())
                );
        }

        [Test]
        [Pending]
        public void Pass_when_checking_multiple_calls_to_same_method_with_one_substitute()
        {
            Received.CallTo(_ignition, x => x.TurnRight())
                    .Then(_engine, x => x.Start())
                    .Then(_engine, x => x.Rev())
                    .Then(_engine, x => x.Rev())
                    .Then(_engine, x => x.Stop());
        }

        [Test]
        [Pending]
        public void Pass_when_checking_multiple_calls_to_same_method_with_multiple_substitutes()
        {
            Received.CallTo(_ignition, x => x.TurnRight())
                .Then(_engine, x => x.Start())
                .Then(_engine, x => x.Rev())
                .Then(_engine, x => x.Rev())
                .Then(_ignition, x => x.TurnLeft())
                .Then(_engine, x => x.Stop())
                .Then(_ignition, x => x.TurnRight())
                .Then(_engine, x => x.Start())
                .Then(_engine, x => x.Stop())
                .Then(_ignition, x => x.TurnLeft());

        }

        [SetUp]
        public void SetUp()
        {
            _engine = Substitute.For<IEngine>();
            _ignition = Substitute.For<IIgnition>();

            _ignition.TurnRight();
            _engine.Start();
            _engine.Rev();
            _engine.Rev();
            _ignition.TurnLeft();
            _engine.Stop();

            _ignition.TurnRight();
            _engine.Start();
            _engine.Stop();
            _ignition.TurnLeft();
        }
    }

    public static class Received
    {
        public static ReceivedCall CallTo<T>(T substitute, Expression<Action<T>> action) where T : class
        {
            var receivedCall = substitute.ReceivedCalls(action).First();

            return new ReceivedCall(receivedCall);
        }
    }

    public class ReceivedCall
    {
        private readonly List<ICall> _receivedCalls;

        public ReceivedCall(ICall call)
        {
            _receivedCalls = new List<ICall> {call};
        }

        public ReceivedCall Then<T>(T substitute, Expression<Action<T>> action) where T : class 
        {
            var receivedCalls = substitute.ReceivedCalls(action).Except(_receivedCalls);

            if(!receivedCalls.Any())
            {
                throw new CallSequenceNotFound();
            }

            if (receivedCalls.First().CallSequenceId <= _receivedCalls.Last().CallSequenceId)
            {
                throw new CallSequenceNotFound();
            }

            _receivedCalls.Add(receivedCalls.First());

            return this;
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