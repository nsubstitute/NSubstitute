using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Exceptions;
using NUnit.Framework;

#pragma warning disable CS0618 // Type or member is obsolete

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ArgumentMatchingCompat
{
    private ISomething _something;

    [Test]
    public void Return_result_for_any_argument()
    {
        _something.Echo(Arg.Compat.Any<int>()).Returns("anything");

        Assert.That(_something.Echo(1), Is.EqualTo("anything"), "First return");
        Assert.That(_something.Echo(2), Is.EqualTo("anything"), "Second return");
    }

    [Test]
    public void Return_result_for_specific_argument()
    {
        _something.Echo(Arg.Compat.Is(3)).Returns("three");
        _something.Echo(4).Returns("four");

        Assert.That(_something.Echo(3), Is.EqualTo("three"), "First return");
        Assert.That(_something.Echo(4), Is.EqualTo("four"), "Second return");
    }

    [Test]
    public void Return_result_for_argument_matching_predicate()
    {
        _something.Echo(Arg.Compat.Is<int>(x => x <= 3)).Returns("small");
        _something.Echo(Arg.Compat.Is<int>(x => x > 3)).Returns("big");

        Assert.That(_something.Echo(1), Is.EqualTo("small"), "First return");
        Assert.That(_something.Echo(4), Is.EqualTo("big"), "Second return");
    }

    [Test]
    public void Should_not_match_when_arg_matcher_throws()
    {
        _something.Say(Arg.Compat.Is<string>(x => x.Length < 2)).Returns("?");

        Assert.That(_something.Say("e"), Is.EqualTo("?"));
        Assert.That(_something.Say("eh"), Is.EqualTo(string.Empty));
        Assert.That(_something.Say(null), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Return_result_with_only_one_matcher_for_that_type()
    {
        _something.Funky(Arg.Compat.Any<float>(), 12, "Lots", null).Returns(42);

        Assert.That(_something.Funky(123.456f, 12, "Lots", null), Is.EqualTo(42));
        Assert.That(_something.Funky(0.0f, 12, "Lots", null), Is.EqualTo(42));
        Assert.That(_something.Funky(0.0f, 11, "Lots", null), Is.EqualTo(0));
    }

    [Test]
    public void Return_result_for_property_argument()
    {
        _something.SomeProperty = 2;
        _something.Echo(_something.SomeProperty).Returns("two");

        Assert.That(_something.Echo(1), Is.EqualTo(""), "First return");
        Assert.That(_something.Echo(2), Is.EqualTo("two"), "Second return");
    }

    [Test]
    public void Received_for_any_argument()
    {
        _something.Echo(7);

        _something.Received().Echo(Arg.Compat.Any<int>());
    }

    [Test]
    public void Received_for_specific_argument()
    {
        _something.Echo(3);

        _something.Received().Echo(Arg.Compat.Is(3));
    }

    [Test]
    public void Received_for_argument_matching_predicate()
    {
        _something.Echo(7);

        _something.Received().Echo(Arg.Compat.Is<int>(x => x > 3));
    }

    [Test]
    public void Received_for_only_one_matcher_for_that_type()
    {
        _something.Funky(123.456f, 12, "Lots", null);

        _something.Received().Funky(Arg.Compat.Any<float>(), 12, "Lots", null);
    }

    [Test]
    public void Received_for_async_method_can_be_awaited()
    {
        TestReceivedAsync().Wait();
    }

    private async Task TestReceivedAsync()
    {
        await _something.Async();
        await _something.Received().Async();
    }

    [Test]
    public void DidNotReceive_for_async_method_can_be_awaited()
    {
        TestDidNotReceiveAsync().Wait();
    }

    private async Task TestDidNotReceiveAsync()
    {
        await _something.DidNotReceive().Async();
    }

    [Test]
    public void Resolve_potentially_ambiguous_matches_by_checking_for_non_default_argument_values()
    {
        _something.Add(10, Arg.Compat.Any<int>()).Returns(1);

        Assert.That(_something.Add(10, 5), Is.EqualTo(1));
    }

    [Test]
    public void Received_should_compare_elements_for_params_arguments()
    {
        const string first = "first";
        const string second = "second";
        _something.WithParams(1, first, second);

        _something.Received().WithParams(1, first, second);
        _something.Received().WithParams(1, Arg.Compat.Any<string>(), second);
        _something.Received().WithParams(1, first, Arg.Compat.Any<string>());
        _something.Received().WithParams(1, [first, second]);
        _something.Received().WithParams(1, Arg.Compat.Any<string[]>());
        _something.Received().WithParams(1, Arg.Compat.Is<string[]>(x => x.Length == 2));
        _something.DidNotReceive().WithParams(2, first, second);
        _something.DidNotReceive().WithParams(2, first, Arg.Compat.Any<string>());
        _something.DidNotReceive().WithParams(1, first, first);
        _something.DidNotReceive().WithParams(1, null);
        _something.DidNotReceive().WithParams(1, Arg.Compat.Is<string[]>(x => x.Length > 3));
    }

    [Test]
    public void Throw_with_ambiguous_arguments_when_given_an_arg_matcher_and_a_default_arg_value_v1()
    {
        Assert.Throws<AmbiguousArgumentsException>(() =>
           {
               _something.Add(0, Arg.Compat.Any<int>()).Returns(1);
               Assert.Fail("Should not make it here, as it can't work out which arg the matcher refers to." +
                           "If it does this will throw an AssertionException rather than AmbiguousArgumentsException.");
           });
    }

    [Test]
    public void Should_add_list_of_all_pending_specifications_to_ambiguous_exception_message()
    {
        var exception = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            _something.Add(0, Arg.Compat.Is(42)).Returns(1);
        });

        Assert.That(exception.Message, Contains.Substring("42"));
    }

    [Test]
    public void Returns_should_work_with_params()
    {
        _something.WithParams(Arg.Compat.Any<int>(), Arg.Compat.Is<string>(x => x == "one")).Returns("fred");

        Assert.That(_something.WithParams(1, "one"), Is.EqualTo("fred"));
    }

    [Test]
    public void Resolve_setter_arg_matcher_with_more_specific_type_than_member_signature()
    {
        const string value = "some string";
        const string key = "key";

        _something[key] = value;

        _something.Received()[key] = Arg.Compat.Is(value);
    }

    [Test]
    public void Resolve_argument_matcher_for_more_specific_type()
    {
        _something.Anything("Hello");
        _something.Received().Anything(Arg.Compat.Any<string>());
        _something.DidNotReceive().Anything(Arg.Compat.Any<int>());
    }

    [Test]
    public void Set_returns_using_more_specific_type_matcher()
    {
        _something.Anything(Arg.Compat.Is<string>(x => x.Contains("world"))).Returns(123);

        Assert.That(_something.Anything("Hello world!"), Is.EqualTo(123));
        Assert.That(_something.Anything("Howdy"), Is.EqualTo(0));
        Assert.That(_something.Anything(2), Is.EqualTo(0));
    }

    [Test]
    public void Override_subclass_arg_matcher_when_returning_for_any_args()
    {
        _something.Anything(Arg.Compat.Any<string>()).ReturnsForAnyArgs(123);

        Assert.That(_something.Anything(new object()), Is.EqualTo(123));
    }

    [Test]
    public void Nullable_args_null_value()
    {
        _something.WithNullableArg(Arg.Compat.Any<int?>()).ReturnsForAnyArgs(123);

        Assert.That(_something.WithNullableArg(null), Is.EqualTo(123));
    }

    [Test]
    public void Nullable_args_notnull_value()
    {
        _something.WithNullableArg(Arg.Compat.Any<int?>()).ReturnsForAnyArgs(123);

        Assert.That(_something.WithNullableArg(234), Is.EqualTo(123));
    }

    public interface IMethodsWithParamsArgs
    {
        int GetValue(int primary, params int[] others);
    }

    [Test]
    public void Should_fail_with_ambiguous_exception_if_params_boundary_is_crossed_scenario_1()
    {
        var target = Substitute.For<IMethodsWithParamsArgs>();

        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            target.GetValue(0, Arg.Compat.Any<int>()).Returns(42);
        });
    }

    [Test]
    public void Should_fail_with_ambiguous_exception_if_params_boundary_is_crossed_scenario_2()
    {
        var target = Substitute.For<IMethodsWithParamsArgs>();

        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            target.GetValue(Arg.Compat.Any<int>(), 0).Returns(42);
        });
    }

    [Test]
    public void Should_correctly_use_matchers_crossing_the_params_boundary()
    {
        var target = Substitute.For<IMethodsWithParamsArgs>();
        target.GetValue(Arg.Compat.Is(0), Arg.Compat.Any<int>()).Returns(42);

        var result = target.GetValue(0, 100);

        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_more_specifications_than_arguments_scenario_1()
    {
        // This spec will be ignored, however it's good to let user know that test might not work how he expects.
        Arg.Compat.Is(42);

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            _something.Echo(10);
        });
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_more_specifications_than_arguments_scenario_2()
    {
        // This one will be used instead of Arg.Compat.Any<>(), causing the confusion.
        Arg.Compat.Is(42);

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            _something.Echo(Arg.Compat.Any<int>());
        });
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_more_specifications_than_arguments_scenario_3()
    {
        // This spec will be ignored, however it's good to let user know that test might not work how he expects.
        Arg.Compat.Is(42);

        var ex = Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            _something.SomeProperty = 24;
        });
        Assert.That(ex.Message, Contains.Substring("42"));
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_more_specifications_than_arguments_scenario_4()
    {
        _something.SomeProperty = 2;
        // This spec will be ignored, however it's good to let user know that test might not work how he expects.
        Arg.Compat.Is(42);

        var ex = Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            _something.Echo(_something.SomeProperty);
        });
        Assert.That(ex.Message, Contains.Substring("42"));
    }

    [Test]
    public void Redundant_argument_matcher_exception_should_contain_list_of_all_matchers()
    {
        Arg.Compat.Is(42);

        var ex = Assert.Throws<RedundantArgumentMatcherException>(() => { _something.Echo(Arg.Compat.Is(24)); });
        Assert.That(ex.Message, Contains.Substring("42"));
        Assert.That(ex.Message, Contains.Substring("24"));
    }

    [Test]
    public void Should_fail_with_redundant_exceptions_if_arg_matchers_misused()
    {
        var foo = Substitute.For<ISomething>();

        var misused = Arg.Compat.Is("test");

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            foo.Echo(2).Returns("42");
        });
    }

    public delegate string DelegateWithOutParameter(string input, out string result);

    [Test]
    public void Should_recognize_out_parameters_for_delegate_and_match_specification()
    {
        var subs = Substitute.For<DelegateWithOutParameter>();
        string _;
        subs.Invoke(Arg.Compat.Any<string>(), out _).Returns("42");

        var result = subs("foo", out _);
        Assert.That(result, Is.EqualTo("42"));
    }

    [SetUp]
    public void SetUp()
    {
        _something = Substitute.For<ISomething>();
    }
}