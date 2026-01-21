using System;
using NSubstitute.Routing.AutoValues;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoStringProviderSpecs
    {
        AutoStringProvider sut = CreateSubjectUnderTest();

        [Test]
        public void Can_provide_value_for_string()
        {
            Assert.That(sut.CanProvideValueFor(typeof(string)));
        }

        [Test]
        public void Should_return_empty_string_value()
        {
            Assert.That(sut.GetValue(typeof(string)), Is.SameAs(string.Empty));
        }

        [Test]
        public void Can_not_provide_a_value_for_an_int_because_that_would_be_silly()
        {
            Assert.That(sut.CanProvideValueFor(typeof(int)), Is.False);
        }

        private static AutoStringProvider CreateSubjectUnderTest()
        {
            return new AutoStringProvider();
        }
    }
}