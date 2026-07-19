using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

#nullable enable

public class Issue973_MatchingWithNullability
{
    public interface ISomething
    {
        int DoSomething(string s);
        int DoSomethingNullable(string? s);
    }

    [Test]
    public void Match_non_null()
    {
        var sub = Substitute.For<ISomething>();

        sub.DoSomething(Arg.Is<string>(x => x.StartsWith("12"))).Returns(42);

        Assert.That(sub.DoSomething("123"), Is.EqualTo(42));
        Assert.That(sub.DoSomething("abc"), Is.EqualTo(0));
    }

    [Test]
    public void Match_nullable()
    {
        var sub = Substitute.For<ISomething>();

        sub.DoSomethingNullable(Arg.Is<string>(x => x.StartsWith("12"))).Returns(42);
        sub.DoSomethingNullable(Arg.Is<string?>(x => x == null)).Returns(456);

        Assert.That(sub.DoSomethingNullable("123"), Is.EqualTo(42));
        Assert.That(sub.DoSomethingNullable("hi"), Is.EqualTo(0));
        Assert.That(sub.DoSomethingNullable(null), Is.EqualTo(456));
    }
}

#nullable restore
