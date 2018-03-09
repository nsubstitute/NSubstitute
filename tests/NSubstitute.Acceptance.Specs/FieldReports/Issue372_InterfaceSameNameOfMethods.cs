using System.Threading.Tasks;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue372_InterfaceSameNameOfMethods
    {
        public interface A<T>
        {
        }

        public interface B<T>
        {
        }

        public interface X
        {
            Task<K> Foo<K>(B<K> bar);
            Task<K> Foo<K>(A<K> bar);
        }

        [Test]
        [Pending, Explicit]
        public void Should_create_substitute()
        {
            var sut = Substitute.For<X>();
            Assert.NotNull(sut);
        }
    }
}