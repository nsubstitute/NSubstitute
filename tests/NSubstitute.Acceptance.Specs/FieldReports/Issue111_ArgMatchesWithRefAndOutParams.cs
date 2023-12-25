using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

[TestFixture]
public class Issue111_ArgMatchesWithRefAndOutParams
{
    public interface IRefAndOutParams
    {
        bool SomeMethodWithRefArg(ref string str);
        bool SomeMethodWithOutArg(out int str);
    }

    private bool CallMethodWithRefParam(IRefAndOutParams refAndOutParams)
    {
        var test = "hello";
        return refAndOutParams.SomeMethodWithRefArg(ref test);
    }

    private bool CallMethodWithOutParam(IRefAndOutParams refAndOutParams)
    {
        var test = 45;
        return refAndOutParams.SomeMethodWithOutArg(out test);
    }

    [Test]
    public void Match_Any_ref_argument_in_When_and_execute_Do()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        var counter = 0;
        var strArg = Arg.Any<string>();
        substitute.When(x => x.SomeMethodWithRefArg(ref strArg)).Do(x => counter++);
        CallMethodWithRefParam(substitute);
        Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public void Should_not_execute_Do_if_given_ref_argument_with_different_value()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        var counter = 0;
        var strArg = "what";
        substitute.When(x => x.SomeMethodWithRefArg(ref strArg)).Do(x => counter++);
        CallMethodWithRefParam(substitute);
        Assert.That(counter, Is.EqualTo(0));
    }

    [Test]
    public void Match_Any_out_argument_in_When_and_execute_Do()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        var counter = 0;
        var intArg = Arg.Any<int>();
        substitute.When(x => x.SomeMethodWithOutArg(out intArg)).Do(x => counter++);
        CallMethodWithOutParam(substitute);
        Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public void Match_out_argument_with_actual_value_in_When_and_execute_Do()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        var counter = 0;
        var intArg = 45;
        substitute.When(x => x.SomeMethodWithOutArg(out intArg)).Do(x => counter++);
        CallMethodWithOutParam(substitute);
        Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public void Should_not_execute_Do_if_given_out_argument_with_different_value()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        var counter = 0;
        var intArg = 3;
        substitute.When(x => x.SomeMethodWithOutArg(out intArg)).Do(x => counter++);
        CallMethodWithOutParam(substitute);
        Assert.That(counter, Is.EqualTo(0));
    }

    [Test]
    public void Return_given_value_when_matching_any_ref_argument()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        var strArg = Arg.Any<string>();
        substitute.SomeMethodWithRefArg(ref strArg).Returns(true);
        Assert.That(CallMethodWithRefParam(substitute), Is.True);
    }

    [Test]
    public void Return_given_value_when_matching_any_out_argument()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        var intArg = Arg.Any<int>();
        substitute.SomeMethodWithOutArg(out intArg).Returns(true);
        Assert.That(CallMethodWithOutParam(substitute), Is.True);
    }

    [Test]
    public void Check_call_was_received_with_any_ref_argument()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        CallMethodWithRefParam(substitute);
        var strArg = Arg.Any<string>();
        substitute.Received().SomeMethodWithRefArg(ref strArg);
    }

    [Test]
    public void Check_call_was_received_with_any_out_argument()
    {
        var substitute = Substitute.For<IRefAndOutParams>();
        CallMethodWithOutParam(substitute);
        var intArg = Arg.Any<int>();
        substitute.Received().SomeMethodWithOutArg(out intArg);
    }
}
