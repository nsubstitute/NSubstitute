using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class Issue149_ArgMatcherInReturns
{
    public interface ISub1 { Item GetItem(string s); }
    public interface ISub2 { string GetSignature(int i); }
    public class Item { }

    [Test]
    public void MatcherInReturnsShouldThrow()
    {
        var sub2 = Substitute.For<ISub2>();
        Assert.Throws<UnexpectedArgumentMatcherException>(
            () => sub2.GetSignature(1).Returns(Arg.Any<string>()));
    }

    // Original example from https://github.com/nsubstitute/NSubstitute/issues/149
    [Test]
    public void OriginalMatcherInReturnsExample()
    {
        var sub1 = Substitute.For<ISub1>();
        var sub2 = Substitute.For<ISub2>();
        sub1.GetItem(Arg.Any<string>()).Returns(new Item());

        Assert.Throws<UnexpectedArgumentMatcherException>(() =>
            sub2.GetSignature(1).Returns(Arg.Any<string>()) // <-- THIS IS THE PROBLEM
        );

        sub1.GetItem("mystring");

        sub1.ReceivedWithAnyArgs(1).GetItem("mystring");
    }
}