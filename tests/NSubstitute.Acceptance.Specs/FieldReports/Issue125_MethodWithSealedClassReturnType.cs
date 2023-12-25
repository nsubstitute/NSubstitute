using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue125_MethodWithSealedClassReturnType
{
    public sealed class SealedClass { }

    public interface IInterface
    {
        SealedClass MethodWithSealedClassReturnType();
    }

    [Test]
    public void MethodWithSealedClassReturnTypeReturnsCorrectResult()
    {
        var substitute = Substitute.For<IInterface>();
        var expected = new SealedClass();
        substitute.MethodWithSealedClassReturnType().Returns(expected);

        var result = substitute.MethodWithSealedClassReturnType();

        Assert.That(result, Is.EqualTo(expected));
    }
}
