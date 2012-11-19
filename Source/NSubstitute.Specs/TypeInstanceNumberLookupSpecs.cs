using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class TypeInstanceNumberLookupSpecs : ConcernFor<TypeInstanceNumberLookup>
    {
        public override TypeInstanceNumberLookup CreateSubjectUnderTest()
        {
            return new TypeInstanceNumberLookup();
        }

        [Test]
        public void First_instance_of_each_type_should_be_instance_one()
        {
            var foo = new Foo();
            var bar = new Bar();

            Assert.AreEqual(1, sut.GetInstanceNumberFor(foo));
            Assert.AreEqual(1, sut.GetInstanceNumberFor(bar));
        }

        [Test]
        public void Should_number_instances_in_the_order_seen()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            Assert.AreEqual(1, sut.GetInstanceNumberFor(foo1));
            Assert.AreEqual(2, sut.GetInstanceNumberFor(foo2));
            Assert.AreEqual(3, sut.GetInstanceNumberFor(foo3));
        }

        [Test]
        public void Subsequent_calls_for_same_instance_should_return_same_number()
        {
            var foo1 = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            sut.GetInstanceNumberFor(foo1);
            sut.GetInstanceNumberFor(foo2);
            Assert.AreEqual(3, sut.GetInstanceNumberFor(foo3));
            Assert.AreEqual(3, sut.GetInstanceNumberFor(foo3));
            Assert.AreEqual(3, sut.GetInstanceNumberFor(foo3));
        }

        private class Foo { }
        private class Bar { }
    }
}