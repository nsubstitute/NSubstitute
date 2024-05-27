using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class SubbingForConcreteTypesAndMultipleInterfaces
{
    [Test]
    public void Can_sub_for_multiple_interfaces()
    {
        var sub = Substitute.For<IFirst, ISecond>();
        var subAsSecondInterface = (ISecond)sub;

        sub.First();
        subAsSecondInterface.Second();

        sub.Received().First();
        subAsSecondInterface.Received().Second();
    }

    [Test]
    public void Can_sub_for_concrete_type_and_implement_other_interfaces()
    {
        var sub = Substitute.For<One, IFirst>();
        var subAsIFirst = (IFirst)sub;

        sub.Number();
        subAsIFirst.First();

        sub.Received().Number();
        subAsIFirst.Received().First();
    }

    [Test]
    public void Partial_sub()
    {
        var sub = Substitute.For<Partial>();
        sub.Number().Returns(10);
        Assert.That(sub.GetNumberPlusOne(), Is.EqualTo(11));
    }

    [Test]
    [Pending, Explicit]
    public void Working_with_nonvirtual_members_of_partial_sub()
    {
        var sub = Substitute.For<Partial>();
        sub.GetNumberPlusOne().Returns(5);
        //TODO: Should this throw???!?!
    }

    [Test]
    public void Sub_with_constructor_arguments()
    {
        var expectedString = "from ctor";
        var expectedInt = 5;
        var sub = Substitute.For<ClassWithCtorArgs>(expectedString, expectedInt);
        Assert.That(sub.StringFromCtorArg, Is.EqualTo(expectedString));
        Assert.That(sub.IntFromCtorArg, Is.EqualTo(expectedInt));
    }

    [Test]
    public void Sub_for_inherited_interfaces()
    {
        var sub = Substitute.For<IFirstAndSecond>();
        sub.Second();
        sub.Received().Second();
    }

    [Test]
    public void Sub_for_interface_with_inherited_generic_interface()
    {
        var sub = Substitute.For<IInheritFromAGenericInterface>();
        sub.Other().Returns(11);

        var result = sub.Other();

        Assert.That(result, Is.EqualTo(11));
        sub.Received().Other();
    }

    public interface IFirst { int First(); }
    public interface ISecond { int Second(); }
    public interface IFirstAndSecond : IFirst, ISecond { }
    public interface IOtherInterface<T> { T Other(); }
    public interface IInheritFromAGenericInterface : IOtherInterface<int> { };
    public class One { public virtual int Number() { return 1; } }
    public class Two { public virtual int AnotherNumber() { return 2; } }
    public class Partial
    {
        public virtual int Number() { return -1; }
        public int GetNumberPlusOne() { return Number() + 1; }
    }
    public abstract class ClassWithCtorArgs(string s, int a)
    {
        public string StringFromCtorArg { get; set; } = s; public int IntFromCtorArg { get; set; } = a;
    }
}