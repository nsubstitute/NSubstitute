using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class DefaultForTypeSpecs : ConcernFor<DefaultForType>
    {
        [Test]
        public void Should_return_null_for_reference_type()
        {
            Assert.That(sut.GetDefaultFor(typeof(object)), Is.Null);
        }

        [Test]
        public void Should_return_default_for_value_type()
        {
            Assert.That(sut.GetDefaultFor(typeof(int)), Is.EqualTo(default(int)));
        }

        [Test]
        public void Should_return_null_for_void()
        {
            Assert.That(sut.GetDefaultFor(typeof(void)), Is.Null);
        }

        public override DefaultForType CreateSubjectUnderTest()
        {
            return new DefaultForType();
        }
    }
}