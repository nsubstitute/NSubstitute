using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue271_DelegateOutArgument
{
    public delegate void Foo(out int bar);

    [Test]
    public void DelegateReturnsOutParameter()
    {
        var foo = Substitute.For<Foo>();
        int bar;
        foo.When(x => x(out bar)).Do(x => { x[0] = 42; });

        foo(out bar);

        Assert.That(bar, Is.EqualTo(42));
    }
}