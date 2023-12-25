using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports;

public class ArgMatchingWithNestedSubCalls
{
    public interface IHaveAMethod { void Method(int a, string b); }
    public interface IStealArgMatchers
    {
        string StealMatcherBeforeUsedElsewhere { get; }
        string this[int i] { get; }
    }

    [Test]
    public void Use_arg_matcher_then_access_another_sub_without_args_before_call_spec_is_created()
    {
        var sub = Substitute.For<IHaveAMethod>();
        var stealer = Substitute.For<IStealArgMatchers>();

        sub.Method(2, stealer.StealMatcherBeforeUsedElsewhere);

        sub.Received().Method(Arg.Any<int>(), stealer.StealMatcherBeforeUsedElsewhere);
    }

    [Test]
    [Pending, Explicit]
    public void Use_arg_matcher_then_access_another_sub_with_args_before_call_spec_is_created()
    {
        var sub = Substitute.For<IHaveAMethod>();
        var stealer = Substitute.For<IStealArgMatchers>();
        stealer[0].Returns("a");

        sub.Method(2, stealer[0]);

        //This example still blows up because the call to stealer[0] takes the Arg.Any<int>() matcher
        //away. The call router thinks the Arg.Any belongs to that call, not the sub.Method() call.
        sub.Received().Method(Arg.Any<int>(), stealer[0]);
    }
}