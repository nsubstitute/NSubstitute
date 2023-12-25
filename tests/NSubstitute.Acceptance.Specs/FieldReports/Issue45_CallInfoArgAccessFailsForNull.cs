using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue45_CallInfoArgAccessFailsForNull
{
    public interface IAmAnInterface
    {
        bool ThatHasAMethodWithArgs(string s, object o);
    }

    [Test]
    public void Should_be_able_to_find_a_null_arg_by_type()
    {
        string stringArgumentUsed = "";

        var sub = Substitute.For<IAmAnInterface>();
        sub.ThatHasAMethodWithArgs(null, null)
            .ReturnsForAnyArgs(x => { stringArgumentUsed = x.Arg<string>(); return true; });

        sub.ThatHasAMethodWithArgs(null, 42);

        Assert.That(stringArgumentUsed, Is.Null);
    }
}