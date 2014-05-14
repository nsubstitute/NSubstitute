using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    [TestFixture]
    public class Issue152_AnyArgsAndGenerics
    {
        public interface IInterfaceWithGenericMethod
        {
            int GenericMethod<T>(T arg);
        }

        [Test]
        [Pending, Explicit]
        public void WhenForAnyArgsTest()
        {
            var sub = Substitute.For<IInterfaceWithGenericMethod>();
            var i = 0;

            sub.WhenForAnyArgs(x => x.GenericMethod<int>(Arg.Any<int>())).Do(x => i++);  // Specifying for int

            sub.GenericMethod<int>(0);
            Assert.AreEqual(1, i); // Works for int

            sub.GenericMethod<string>("");
            Assert.AreEqual(2, i); // Fails for string
        }

        [Test]
        [Pending, Explicit]
        public void ReturnsTest()
        {
            var sub = Substitute.For<IInterfaceWithGenericMethod>();

            const int expectedInt = 1;
            sub.GenericMethod(Arg.Any<int>()).ReturnsForAnyArgs(expectedInt); // Specifying for int

            Assert.AreEqual(expectedInt, sub.GenericMethod(0)); // Works for int
            Assert.AreEqual(expectedInt, sub.GenericMethod("")); // Fails for string
        }
    }
}