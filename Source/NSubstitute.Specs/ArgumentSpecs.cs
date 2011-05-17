using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ArgumentSpecs
    {
        interface IFoo { }
        class Foo : IFoo { }

        [Test]
        public void ShouldReturnActualTypeOfArgument()
        {
            var arg = new Argument(typeof(IFoo), new Foo());
            Assert.That(arg.DeclaredType, Is.EqualTo(typeof(IFoo)));
            Assert.That(arg.ActualType, Is.EqualTo(typeof(Foo)));
        }

        [Test]
        public void ForNullValuesShouldReturnDeclaredTypeAsActualType()
        {
            var arg = new Argument(typeof(IFoo), null);
            Assert.That(arg.DeclaredType, Is.EqualTo(typeof(IFoo)));
            Assert.That(arg.ActualType, Is.EqualTo(typeof(IFoo)));
        }
    }
}