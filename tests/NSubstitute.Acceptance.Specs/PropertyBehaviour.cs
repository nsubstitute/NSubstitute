using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class PropertyBehaviour
{
    public interface IFoo
    {
        string Name { get; set; }
        DateTime Now { get; }
        string WriteOnly { set; }
        string this[int i] { get; set; }
        string this[string a, string b] { get; set; }
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
    public void Indexer_properties_should_just_work()
    {
        var foo = Substitute.For<IFoo>();
        foo[2] = "two";
        Assert.That(foo[2], Is.EqualTo("two"));
        Assert.That(foo[0], Is.Not.EqualTo("two"));
    }

    [Test]
    public void Indexer_properties_with_multiple_arguments_should_work_ok_too()
    {
        var foo = Substitute.For<IFoo>();
        foo["one", "two"] = "three";
        Assert.That(foo["one", "two"], Is.EqualTo("three"));
        Assert.That(foo["one", "one"], Is.Not.EqualTo("three"));
    }

    [Test]
    public void Indexer_with_arg_matchers()
    {
        var foo = Substitute.For<IFoo>();

        foo[Arg.Any<int>()] = "test";

        foo.DidNotReceiveWithAnyArgs()[0] = "test";
        Assert.That(foo[1], Is.EqualTo("test"));
        Assert.That(foo[7], Is.EqualTo("test"));
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
        Assert.Throws<ReceivedCallsException>(() => foo.Received().Name = "Other name");
    }

    [Test]
    public void Check_a_property_getter_was_called()
    {
        var foo = Substitute.For<IFoo>();
        _ignored = foo.Name;
        _ignored = foo.Received().Name;
        Assert.Throws<ReceivedCallsException>(() => { _ignored = foo.Received().Now; });
    }

    [Test]
    public void Can_set_a_value_on_a_write_only_property()
    {
        const string somethingToWrite = "high falutin' writable stuff. Yeehaw!";
        var foo = Substitute.For<IFoo>();
        foo.WriteOnly = somethingToWrite;
        foo.Received().WriteOnly = somethingToWrite;
        Assert.Throws<ReceivedCallsException>(() => foo.Received().WriteOnly = "non-fancy writable stuff");
    }
}