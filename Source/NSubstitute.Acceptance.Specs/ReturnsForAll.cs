using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public  class ReturnsForAll
    {
        private IFluentSomething _fluentSomething;

        [SetUp]
        public void SetUp()
        {
            _fluentSomething = Substitute.For<IFluentSomething>();
        }

        [Test]
        public void Return_self_for_single_call()
        {
            _fluentSomething.ReturnsForAll<IFluentSomething>(_fluentSomething);

            Assert.Fail();
        }
    }
}
