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
        public void Check_that_base_is_called()
        {
            _car.CallBaseWhen().Start();
            _car.Start();
            Assert.IsTrue(_car.IsStarted);
        }

        public class Car
        {
            public bool IsStarted { get; set; }
            public virtual void Start()
            {
                IsStarted = true;
            }
        }
    }
}