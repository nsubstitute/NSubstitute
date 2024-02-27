using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NSubstitute.Acceptance.Specs.FieldReports;

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
    public void Should_create_substitute()
    {
        var sut = Substitute.For<X>();
        ClassicAssert.NotNull(sut);
    }
}