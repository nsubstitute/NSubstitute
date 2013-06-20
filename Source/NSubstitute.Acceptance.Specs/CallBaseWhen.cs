using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class CallBaseWhen
    {
        private Car _car;

        [SetUp]
        public void SetUp()
        {
            _car = Substitute.For<Car>();
        }

        [Test]
        public void Check_when_call_was_received()
        {
            _car.CallBaseWhen().Start();
            Assert.IsTrue(_car.IsStarted);
        }

        public class Car
        {
            public bool IsStarted { get; set; }
            public void Start()
            {
                IsStarted = true;
            }
        }
    }
}