using System;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ReturnsForAllFromFunc
    {
        private IFluentSomething _fluentSomething;
        private ISomething _something;

        [SetUp]
        public void SetUp()
        {
            _fluentSomething = Substitute.For<IFluentSomething>();
            _something = Substitute.For<ISomething>();
            _fluentSomething.ReturnsForAll<IFluentSomething>(ci => _fluentSomething);
            _fluentSomething.ReturnsForAll<ISomething>(ci => _something);
        }

        [Test]
        public void Return_self_for_single_call()
        {
            Assert.That(_fluentSomething.Chain(), Is.SameAs(_fluentSomething));
        }

        [Test]
        public void Return_self_for_chained_calls()
        {
           Assert.That(_fluentSomething.Chain().Me().Together(), Is.SameAs(_fluentSomething));
        }

        [Test]
        public void Return_value_that_is_not_chainable()
        {
            Assert.That(_fluentSomething.SorryNoChainingHere(), Is.SameAs(_something));
        }

        [Test]
        public void Return_samce_thing_for_multiple_calls()
        {
            var _first = _fluentSomething.SorryNoChainingHere();
            var _second = _fluentSomething.SorryNoChainingHere();
            Assert.That(_first, Is.SameAs(_something));
            Assert.That(_second, Is.SameAs(_first));
        }

        [Test]
        public void Return_concrete_derived_type()
        {
            var _concreteSomething = new FluentSomething();
            _fluentSomething.ReturnsForAll<IFluentSomething>(_concreteSomething);
            Assert.That(_fluentSomething.Chain(), Is.SameAs(_concreteSomething));
        }
    }
}