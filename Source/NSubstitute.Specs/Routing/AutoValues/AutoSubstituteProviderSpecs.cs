using System;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoSubstituteProviderSpecs : ConcernFor<AutoSubstituteProvider>
    {
        private ISubstituteFactory _substituteFactory;

        [Test]
        public void Can_provide_value_for_interface()
        {
            Assert.That(sut.CanProvideValueFor(typeof(IFoo)));
        }

        [Test]
        public void Can_provide_value_for_delegates()
        {
            Assert.That(sut.CanProvideValueFor(typeof(Func<int>)));
        }

        [Test]
        public void Should_create_substitute_for_type()
        {
            var autoValue = new object();
            _substituteFactory.stub(x => x.Create(new[] { typeof(IFoo) }, new object[0])).Return(autoValue);

            Assert.That(sut.GetValue(typeof(IFoo)), Is.SameAs(autoValue));
        }

        public override void Context()
        {
            _substituteFactory = mock<ISubstituteFactory>();
        }

        public override AutoSubstituteProvider CreateSubjectUnderTest()
        {
            return new AutoSubstituteProvider(_substituteFactory);
        }
    }
}