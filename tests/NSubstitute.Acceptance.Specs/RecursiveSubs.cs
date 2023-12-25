using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class RecursiveSubs
{
    public interface IContext
    {
        IRequest Request { get; }
        IRequest GetRequest();
        IRequest GetRequest(int number);
    }
    public interface IRequest
    {
        IIdentity Identity { get; }
        IIdentity GetIdentity();
    }
    public interface IIdentity { string Name { get; set; } }

    [Test]
    public void Set_a_return_value_on_a_substitute_of_a_substitute_of_a_substitute()
    {
        const string name = "My pet fish Eric";

        var context = Substitute.For<IContext>();
        context.Request.Identity.Name.Returns(name);

        Assert.That(context.Request.Identity.Name, Is.EqualTo(name));
    }

    [Test]
    public void Recursively_generated_subs_should_return_the_same_sub_each_time()
    {
        var context = Substitute.For<IContext>();
        var identity = context.GetRequest().GetIdentity();
        var identityAgain = context.GetRequest().GetIdentity();
        Assert.That(identity, Is.SameAs(identityAgain));
    }

    [Test]
    public void Can_override_a_recursively_generated_sub()
    {
        var newIdentity = Substitute.For<IIdentity>();
        var context = Substitute.For<IContext>();

        var autoCreatedIdentity = context.GetRequest().GetIdentity();
        context.GetRequest().GetIdentity().Returns(newIdentity);

        Assert.That(context.GetRequest().GetIdentity(), Is.Not.SameAs(autoCreatedIdentity));
        Assert.That(context.GetRequest().GetIdentity(), Is.SameAs(newIdentity));
    }

    [Test]
    public void Set_a_property_on_a_recursively_created_sub()
    {
        var context = Substitute.For<IContext>();
        context.Request.Identity.Name = "Eric";
        Assert.That(context.Request.Identity.Name, Is.EqualTo("Eric"));
    }

    [Test]
    public void Recursively_generate_for_methods_that_take_arguments()
    {
        var context = Substitute.For<IContext>();
        context.GetRequest(10).Identity.Name = "Eric";
        Assert.That(context.GetRequest(10).Identity.Name, Is.EqualTo("Eric"));
        Assert.That(context.GetRequest(9).Identity.Name, Is.Not.EqualTo("Eric"));
    }

    [Test]
    public void Argument_matchers_with_recursive_stubs()
    {
        var context = Substitute.For<IContext>();
        context.GetRequest(Arg.Is<int>(x => x < 10)).Identity.Name = "Eric";

        Assert.That(context.GetRequest(1).Identity.Name, Is.EqualTo("Eric"));
        Assert.That(context.GetRequest(2).Identity.Name, Is.EqualTo("Eric"));
        Assert.That(context.GetRequest(99).Identity.Name, Is.Not.EqualTo("Eric"));
    }
}