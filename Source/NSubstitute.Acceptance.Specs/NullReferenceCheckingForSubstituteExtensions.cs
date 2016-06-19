using System;
using NSubstitute.ClearExtensions;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class NullReferenceCheckingForSubstituteExtensions
    {
        IEngine _engine;

        [SetUp]
        public void SetUp()
        {
            _engine = null;
        }

        [Test]
        public void Call_to_received()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.Received().Rev());
        }

        [Test]
        public void Call_to_did_not_receive()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.DidNotReceive().Rev());
        }

        [Test]
        public void Call_to_received_with_any_args()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.ReceivedWithAnyArgs().Rev());
        }

        [Test]
        public void Call_to_did_not_receive_with_any_args()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.DidNotReceiveWithAnyArgs().Rev());
        }

        [Test]
        public void Call_to_when()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.When(x => x.Rev()).Do(x => { }));
        }

        [Test]
        public void Call_to_clear_received_calls()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.ClearReceivedCalls());
        }

        [Test]
        public void Call_to_clear_substitute()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.ClearSubstitute(ClearOptions.CallActions));
        }

        [Test]
        public void Call_to_received_calls()
        {
            Assert.Throws<NullSubstituteReferenceException>(() => _engine.ReceivedCalls());
        }

        public interface IEngine
        {
            void Start();
            void Rev();
            void Stop();
            void Idle();
            void RevAt(int rpm);
            void FillPetrolTankTo(int percent);
            float GetCapacityInLitres();
            event Action Started;
        }
    }
}