using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class PropertyBehaviour
    {
        public interface IFoo
        {
            string Name { get; set; }
            DateTime Now { get; }
            string WriteOnly { set; }
            string this[int i] { get; set; }
        }

        protected object _ignored;

        [Test]
        public void Properties_just_work()
        {
            var foo = Substitute.For<IFoo>();
            foo.Name = "This name";
            Assert.That(foo.Name, Is.EqualTo("This name"));
        }

        [Test]
        [Pending]
        public void Indexer_properties_should_just_work()
        {
            var foo = Substitute.For<IFoo>();
            foo[2] = "two";
            Assert.That(foo[2], Is.EqualTo("two"));
        }

        [Test]
        public void Make_a_readonly_property_return_a_specific_value()
        {
            var foo = Substitute.For<IFoo>();
            var specificDate = new DateTime(2010, 1, 2, 3, 4, 5);
            foo.Now.Returns(specificDate);
            Assert.That(foo.Now, Is.EqualTo(specificDate));
        }

        [Test]
        public void Check_a_property_setter_was_called()
        {
            var foo = Substitute.For<IFoo>();
            foo.Name = "This name";
            foo.Received().Name = "This name";
            Assert.Throws<CallNotReceivedException>(() => foo.Received().Name = "Other name");
        }

        [Test]
        public void Check_a_property_getter_was_called()
        {
            var foo = Substitute.For<IFoo>();
            _ignored = foo.Name;
            _ignored = foo.Received().Name;
            Assert.Throws<CallNotReceivedException>(() => { _ignored = foo.Received().Now; });
        }

        [Test]
        public void Can_set_a_value_on_a_write_only_property()
        {
            const string somethingToWrite = "high falutin' writable stuff. Yeehaw!";
            var foo = Substitute.For<IFoo>();
            foo.WriteOnly = somethingToWrite;
            foo.Received().WriteOnly = somethingToWrite;
            Assert.Throws<CallNotReceivedException>(() => foo.Received().WriteOnly = "non-fancy writable stuff");
        }
    }
}