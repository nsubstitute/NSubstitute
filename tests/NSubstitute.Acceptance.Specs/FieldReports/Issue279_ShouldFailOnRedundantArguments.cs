using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue279_ShouldFailOnRedundantArguments
{
    public interface IFoo
    {
        int Blah(double s);
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_matcher_is_not_used_due_to_implicit_cast_scenario_1()
    {
        var foo = Substitute.For<IFoo>();

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            foo.Blah(Arg.Any<int>()).Returns(42);
        });
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_matcher_is_not_used_due_to_implicit_cast_scenario_2()
    {
        var foo = Substitute.For<IFoo>();

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            // Fails because Is<T>() type is deduced to int, so specifier is not matched later.
            foo.Blah(Arg.Is(10)).Returns(42);
        });
    }
}