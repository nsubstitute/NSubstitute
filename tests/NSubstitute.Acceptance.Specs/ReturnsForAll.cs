using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ReturnsForAll
{
    private IFluentSomething _fluentSomething;
    private ISomething _something;

    [SetUp]
    public void SetUp()
    {
        _fluentSomething = Substitute.For<IFluentSomething>();
        _something = Substitute.For<ISomething>();
        _fluentSomething.ReturnsForAll(_fluentSomething);
        _fluentSomething.ReturnsForAll(_something);
    }

    [Test]
    public void Return_self_for_single_call()
    {
        Assert.That(_fluentSomething.Chain(), Is.SameAs(_fluentSomething));
    }

    [Test]
    public void Return_self_for_chained_calls()
    {
        Assert.That(_fluentSomething.Chain().Me().Together(), Is.SameAs(_fluentSomething));
    }

    [Test]
    public void Return_value_that_is_not_chainable()
    {
        Assert.That(_fluentSomething.SorryNoChainingHere(), Is.SameAs(_something));
    }

    [Test]
    public void Return_same_thing_for_multiple_calls()
    {
        var first = _fluentSomething.SorryNoChainingHere();
        var here = _fluentSomething.SorryNoChainingHere();
        Assert.That(first, Is.SameAs(_something));
        Assert.That(here, Is.SameAs(first));
    }

    [Test]
    public void Return_concrete_derived_type()
    {
        var concreteSomething = new FluentSomething();
        _fluentSomething.ReturnsForAll<IFluentSomething>(concreteSomething);
        Assert.That(_fluentSomething.Chain(), Is.SameAs(concreteSomething));
    }

    [Test]
    public void Specific_returns_trumps_returnsforall()
    {
        var thing1 = new FluentSomething();
        var thing2 = new FluentSomething();
        _fluentSomething.ReturnsForAll<IFluentSomething>(thing1);
        _fluentSomething.Chain().Returns(thing2);
        Assert.That(_fluentSomething.Chain(), Is.SameAs(thing2));
        Assert.That(_fluentSomething.Me(), Is.SameAs(thing1));
    }
}