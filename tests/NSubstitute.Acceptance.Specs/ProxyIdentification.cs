using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class ProxyIdentification
{
    [Test]
    public void ProxyIdShouldContainTypeName()
    {
        var sut = Substitute.For<IInterface>();

        var formatted = sut.ToString();

        Assert.That(formatted, Contains.Substring(nameof(IInterface)));
    }

    [Test]
    public void IdContainsPrimaryTypeOnly()
    {
        var proxy = Substitute.For<IInterface, IOtherInterface>();

        var id = proxy.ToString();

        Assert.That(id, Does.Not.Contain(nameof(IOtherInterface)));
    }

    [Test]
    public void ProxyIdShouldBeUnique()
    {
        var proxy1 = Substitute.For<IInterface>();
        var proxy2 = Substitute.For<IInterface>();

        var id1 = proxy1.ToString();
        var id2 = proxy2.ToString();

        Assert.That(id1, Is.Not.EqualTo(id2));
    }

    [Test]
    public void ShouldGenerateIdForTypeWithDefaultToString()
    {
        var proxy1 = Substitute.For<ClassWithDefaultToString>();
        var proxy2 = Substitute.For<ClassWithDefaultToString>();

        var id1 = proxy1.ToString();
        var id2 = proxy2.ToString();

        Assert.That(id1, Is.Not.EqualTo(id2));
    }

    [Test]
    public void ShouldNotGenerateIdForTypesWithOverloadedToString()
    {
        var proxy = Substitute.For<ClassWithOverloadedToString>();

        var id = proxy.ToString();

        Assert.That(id, Is.EqualTo(ClassWithOverloadedToString.ToStringValue));
    }

    [Test]
    public void ShouldContainFullGenericTypeName()
    {
        var proxy = Substitute.For<IGenericInterface<byte>>();

        var id = proxy.ToString();

        Assert.That(id, Contains.Substring("IGenericInterface<Byte>"));
    }

    public interface IInterface
    {
        int SomeMethod();
    }

    public interface IOtherInterface
    {
        int SomeMethod();
    }

    public interface IGenericInterface<T>
    {
        T SomeMethod();
    }

    public class ClassWithDefaultToString
    {
        public virtual int SomeMethod() => 0;
    }

    public class ClassWithOverloadedToString
    {
        public const string ToStringValue = "42";

        public virtual int SomeMethod() => 0;
        public override string ToString() => ToStringValue;
    }
}