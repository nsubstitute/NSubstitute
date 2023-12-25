using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class ArgMatchingWithValueTypeArgSpecsForObjectArguments
{
    public interface IInterface
    {
        void DoSomething(object input);
        void DoSomething(object first, object second, object third);
    }

    [Test]
    public void Match_value_type_arg_spec_provided_for_object_argument()
    {
        var sub = Substitute.For<IInterface>();

        sub.DoSomething(Guid.NewGuid());

        sub.Received().DoSomething(Arg.Any<Guid>());
    }

    [Test]
    public void Value_type_arg_spec_should_filter_out_call_made_passing_in_an_object()
    {
        var sub = Substitute.For<IInterface>();

        sub.DoSomething(new object());

        sub.DidNotReceive().DoSomething(Arg.Any<Guid>());
    }

    [Test]
    public void Checking_some_multiple_argument_cases()
    {
        var anObject = new object();

        var sub = Substitute.For<IInterface>();

        sub.DoSomething(Guid.NewGuid(), 123, anObject);

        sub.Received().DoSomething(Arg.Any<Guid>(), Arg.Any<int>(), anObject);
        sub.Received().DoSomething(Arg.Any<object>(), Arg.Any<int>(), anObject);
        sub.Received().DoSomething(Arg.Any<object>(), Arg.Any<object>(), anObject);
        sub.Received().DoSomething(Arg.Any<object>(), Arg.Any<object>(), Arg.Any<object>());
        sub.Received().DoSomething(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<object>());
        sub.Received().DoSomething(Arg.Any<Guid>(), 123, Arg.Any<object>());

        sub.DidNotReceive().DoSomething(Arg.Any<int>(), Arg.Any<int>(), anObject);
        sub.DidNotReceive().DoSomething(Arg.Any<int>(), Arg.Any<Guid>(), anObject);
    }
}