using System;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoArrayProviderSpecs : ConcernFor<AutoArrayProvider>
    {
        [Test]
        public void Provides_empty_array_value_for_arrays()
        {
            Assert.That(sut.GetValue(typeof(string[])), Is.EqualTo(Maybe.Just(new string[0])));
            Assert.That(sut.GetValue(typeof(int[])), Is.EqualTo(Maybe.Just(new int[0])));
        }

        [Test]
        public void Provides_no_value_for_non_arrays()
        {
            Assert.That(sut.GetValue(typeof(string)), Is.EqualTo(Maybe.Nothing<object>()));
            Assert.That(sut.GetValue(typeof(int)), Is.EqualTo(Maybe.Nothing<object>()));
        }

        public override AutoArrayProvider CreateSubjectUnderTest()
        {
            return new AutoArrayProvider();
        }
    }
}