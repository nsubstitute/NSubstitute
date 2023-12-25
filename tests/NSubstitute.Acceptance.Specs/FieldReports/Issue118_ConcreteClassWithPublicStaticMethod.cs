using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

class Issue118_ConcreteClassWithPublicStaticMethod
{
    [Test]
    public void Interface_returning_concrete_without_static_member_should_return_substitute_by_default()
    {
        var sub = Substitute.For<IInterfaceReturningConcreteWithoutPublicStaticMethod>();

        var returnedClass = sub.AMethod();

        Assert.That(returnedClass, Is.InstanceOf<ConcreteWithoutPublicStaticMethod>());
    }

    [Test]
    public void Interface_returning_concrete_without_static_member_should_return_object_if_instructed_to()
    {
        var sub = Substitute.For<IInterfaceReturningConcreteWithoutPublicStaticMethod>();
        var obj = new ConcreteWithoutPublicStaticMethod();
        sub.AMethod().Returns(obj);

        var returnedClass = sub.AMethod();

        Assert.That(returnedClass, Is.SameAs(obj));
    }

    [Test]
    public void Interface_returning_concrete_with_static_member_should_return_substitute_by_default()
    {
        var sub = Substitute.For<IInterfaceReturningConcreteWithPublicStaticMethod>();

        var returnedClass = sub.AMethod();

        Assert.That(returnedClass, Is.InstanceOf<ConcreteWithPublicStaticMethod>());
    }

    [Test]
    public void Interface_returning_concrete_with_static_member_should_return_object_if_instructed_to()
    {
        var sub = Substitute.For<IInterfaceReturningConcreteWithPublicStaticMethod>();
        var obj = new ConcreteWithPublicStaticMethod();
        sub.AMethod().Returns(obj);

        var returnedClass = sub.AMethod();

        Assert.That(returnedClass, Is.SameAs(obj));
    }

    [Test]
    public void Substitute_of_concrete_with_static_member_should_allow_setup_of_return_value()
    {
        const string value = "test";
        var sub = Substitute.For<ConcreteWithPublicStaticMethod>();
        sub.AProperty.Returns(value);

        var returnedValue = sub.AProperty;

        Assert.That(returnedValue, Is.EqualTo(value));
    }
}

public class ConcreteWithPublicStaticMethod
{
    public virtual string AProperty { get; set; }
    public static void AStaticMethod() { }
}

public class ConcreteWithoutPublicStaticMethod
{
    public virtual string AProperty { get; set; }
}

public interface IInterfaceReturningConcreteWithPublicStaticMethod
{
    ConcreteWithPublicStaticMethod AMethod();
}

public interface IInterfaceReturningConcreteWithoutPublicStaticMethod
{
    ConcreteWithoutPublicStaticMethod AMethod();
}
