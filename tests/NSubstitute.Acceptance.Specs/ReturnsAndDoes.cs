using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class ReturnsAndDoes
{
    public interface IFoo { IBar GetBar(int i); }
    public interface IBar { }

    private IFoo sub;

    [SetUp]
    public void SetUp()
    {
        sub = Substitute.For<IFoo>();
    }

    [Test]
    public void Set_callback_via_extension_method()
    {
        var bar = Substitute.For<IBar>();
        var wasCalled = false;
        sub.GetBar(2).Returns(bar).AndDoes(x => wasCalled = true);
        sub.GetBar(1);

        Assert.That(wasCalled, Is.False);
        var result = sub.GetBar(2);
        Assert.That(result, Is.EqualTo(bar));
        Assert.That(wasCalled, Is.True);
    }

    [Test]
    public void Add_multiple_callbacks_and_ignore_args()
    {
        var bar = Substitute.For<IBar>();
        var wasCalled = false;
        var argsUsed = new List<int>();
        sub.GetBar(2)
           .ReturnsForAnyArgs(bar)
           .AndDoes(x => wasCalled = true)
           .AndDoes(x => argsUsed.Add(x.Arg<int>()));

        var result1 = sub.GetBar(42);
        var result2 = sub.GetBar(9999);
        Assert.That(wasCalled, Is.True);
        Assert.That(argsUsed, Is.EquivalentTo(new[] { 42, 9999 }));
        Assert.That(bar, Is.SameAs(result1));
        Assert.That(bar, Is.SameAs(result2));
    }

    [Test]
    public void Interaction_with_other_callback_methods()
    {
        var calledViaWhenDo = false;
        var calledViaArgDo = false;
        var calledViaAndDoes = false;

        sub.GetBar(2).Returns(Substitute.For<IBar>()).AndDoes(x => calledViaAndDoes = true);
        sub.When(x => x.GetBar(2)).Do(x => calledViaWhenDo = true);
        sub.GetBar(Arg.Do<int>(x => calledViaArgDo = true));

        Assert.That(calledViaAndDoes && calledViaArgDo && calledViaWhenDo, Is.False);

        sub.GetBar(2);

        Assert.That(calledViaAndDoes);
        Assert.That(calledViaArgDo);
        Assert.That(calledViaWhenDo);
    }
}