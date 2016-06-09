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
        public void Provides_no_value_for_non_string()
        {
            Assert.That(sut.GetValue(typeof(int)), Is.EqualTo(Maybe.Nothing<object>()));
        }

        [Test]
        public void Provides_value_for_string()
        {
            Assert.That(sut.GetValue(typeof(string)), Is.EqualTo(Maybe.Just<object>("")));
        }

        public override AutoStringProvider CreateSubjectUnderTest()
        {
            return new AutoStringProvider();
        }
    }
}