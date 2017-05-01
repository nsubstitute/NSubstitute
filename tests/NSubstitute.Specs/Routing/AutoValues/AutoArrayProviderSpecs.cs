using System;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoArrayProviderSpecs : ConcernFor<AutoArrayProvider>
    {
        [Test]
        public void Can_provide_value_for_arrays()
        {
            Assert.That(sut.CanProvideValueFor(typeof(int[])));
        }

        [Test]
        public void Can_not_provide_value_for_non_arrays()
        {
            Assert.False(sut.CanProvideValueFor(typeof(string)));
            Assert.False(sut.CanProvideValueFor(typeof(int)));
        }

        [Test]
        public void Provides_empty_array_value()
        {
            var array = sut.GetValue(typeof(int[]));
            Assert.That(array, Is.Not.Null);
            Assert.That(array, Is.Empty);
        }

        public override AutoArrayProvider CreateSubjectUnderTest()
        {
            return new AutoArrayProvider();
        }
    }
}