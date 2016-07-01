using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class DynamicCalls
    {
#if (NET4 || NET45 || NETSTANDARD1_5)
        public interface IInterface
        {
            dynamic ReturnsDynamic(string a);
            dynamic ReturnsAndGetsDynamic(dynamic a);
            int GetsDynamic(dynamic a);
            dynamic DynamicProperty { get; set; }
        }

        [Test]
        [Pending, Explicit]
        public void MethodGetsDynamicAndSpecifiedWithDynamic()
        {
            var sut = Substitute.For<IInterface>();
            // Fails
            // Because dynamic typing doesn't support extension methods
            // We can't do much here, I see only two options:
            // 1. Good documentation. Tell people to use exact type instead of dynamic or use static Returns method.
            // 2. Try to catch calls with dynamic typing and throw descriptive exception. Can be made via StackTrace. A bit hacky and risky.
            // TBD
            sut.GetsDynamic(Arg.Any<dynamic>()).Returns(1);

            dynamic expando = new System.Dynamic.ExpandoObject();
            var result = sut.GetsDynamic(expando);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void MethodGetsDynamicButSpecifiedWithExplicitType()
        {
            var sut = Substitute.For<IInterface>();
            sut.GetsDynamic(Arg.Any<object>()).Returns(1);

            dynamic expando = new System.Dynamic.ExpandoObject();
            var result = sut.GetsDynamic(expando);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void DynamicProperty()
        {
            var sut = Substitute.For<IInterface>();
            sut.DynamicProperty.Returns(1);

            var result = sut.DynamicProperty;

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void MethodGetsDynamicAsAnArgumentAndReturnsDynamic()
        {
            var sut = Substitute.For<IInterface>();
            sut.ReturnsAndGetsDynamic(Arg.Any<dynamic>()).Returns(1);

            dynamic expando = new System.Dynamic.ExpandoObject();
            var result = sut.ReturnsAndGetsDynamic(expando);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void MethodReturnsDynamic()
        {
            var sut = Substitute.For<IInterface>();
            sut.ReturnsDynamic(Arg.Any<string>()).Returns(1);

            var result = sut.ReturnsDynamic("");

            Assert.That(result, Is.EqualTo(1));
        }
#endif
    }
}