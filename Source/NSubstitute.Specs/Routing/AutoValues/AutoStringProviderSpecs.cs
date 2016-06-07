using System;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoStringProviderSpecs : ConcernFor<AutoStringProvider>
    {
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

        [Test]
        public void Provides_no_value_for_non_string()
        {
            Assert.That(((IMaybeAutoValueProvider)sut).GetValue(typeof(int)), Is.EqualTo(Maybe.Nothing<object>()));
        }

        [Test]
        public void Provides_value_for_string()
        {
            Assert.That(((IMaybeAutoValueProvider)sut).GetValue(typeof(string)), Is.EqualTo(Maybe.Just<object>("")));
        }

        public override AutoStringProvider CreateSubjectUnderTest()
        {
            return new AutoStringProvider();
        }
    }
}