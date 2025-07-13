using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class ArgAnyStringRegression
{
    public interface IFoo { string Bar(string a, double b); }

    [Test]
    public void Stub_any_string_and_call_with_null()
    {
        var foo = Substitute.For<IFoo>();
        foo.Bar(Arg.Any<string>(), Arg.Any<double>()).ReturnsForAnyArgs("hello");

        Assert.That(foo.Bar(null, 0), Is.EqualTo("hello"));
    }
}