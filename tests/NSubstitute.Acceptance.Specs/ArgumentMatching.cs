using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ArgumentMatching
{
    private ISomething _something;

    [Test]
    public void Return_result_for_any_argument()
    {
        _something.Echo(Arg.Any<int>()).Returns("anything");

        Assert.That(_something.Echo(1), Is.EqualTo("anything"), "First return");
        Assert.That(_something.Echo(2), Is.EqualTo("anything"), "Second return");
    }

    [Test]
    public void Return_result_for_specific_argument()
    {
        _something.Echo(Arg.Is(3)).Returns("three");
        _something.Echo(4).Returns("four");

        Assert.That(_something.Echo(3), Is.EqualTo("three"), "First return");
        Assert.That(_something.Echo(4), Is.EqualTo("four"), "Second return");
    }

    [Test]
    public void Return_result_for_argument_matching_predicate()
    {
        _something.Echo(Arg.Is<int>(x => x <= 3)).Returns("small");
        _something.Echo(Arg.Is<int>(x => x > 3)).Returns("big");

        Assert.That(_something.Echo(1), Is.EqualTo("small"), "First return");
        Assert.That(_something.Echo(4), Is.EqualTo("big"), "Second return");
    }

    [Test]
    public void Should_not_match_when_arg_matcher_throws()
    {
        _something.Say(Arg.Is<string>(x => x.Length < 2)).Returns("?");

        Assert.That(_something.Say("e"), Is.EqualTo("?"));
        Assert.That(_something.Say("eh"), Is.EqualTo(string.Empty));
        Assert.That(_something.Say(null), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Should_match_value_types_by_content()
    {
        const int intToMatch = 123;
        const int identicalInt = 123;
        _something.Echo(Arg.Is(intToMatch)).Returns("matching int");

        Assert.That(_something.Echo(intToMatch), Is.EqualTo("matching int"));
        Assert.That(_something.Echo(identicalInt), Is.EqualTo("matching int"));

        var dateToMatch = new DateTime(2021, 10, 22);
        var identicalDate = new DateTime(2021, 10, 22);
        _something.Anything(dateToMatch).Returns(20211022);

        Assert.That(_something.Anything(dateToMatch), Is.EqualTo(20211022));
        Assert.That(_something.Anything(identicalDate), Is.EqualTo(20211022));
    }

    [Test]
    public void Should_match_strings_by_content()
    {
        const string stringToMatch = "hello";
        _something.Say(Arg.Is(stringToMatch)).Returns("hi");

        Assert.That(_something.Say(stringToMatch), Is.EqualTo("hi"));
        Assert.That(_something.Say("hello"), Is.EqualTo("hi"));
    }

    [Test]
    public void Should_match_nullable_ref_types_by_content()
    {
#nullable enable
        SomeClass? nullClassToMatch = null;
        List<int>? nullList = null;
        _something.Anything(Arg.Is(nullClassToMatch)).Returns(456);

        Assert.That(_something.Anything(nullClassToMatch), Is.EqualTo(456));
        Assert.That(_something.Anything(nullList), Is.EqualTo(456));
#nullable disable
    }

    [Test]
    public void Should_match_non_string_non_record_ref_types_by_reference()
    {
        var listToMatch = new List<int> { 1, 2 };
        _something.Anything(Arg.Is(listToMatch)).Returns(123);

        Assert.That(_something.Anything(listToMatch), Is.EqualTo(123));
        Assert.That(_something.Anything(new List<int> { 1, 2 }), Is.EqualTo(0));

        var classToMatch = new SomeClass();
        _something.Anything(Arg.Is(classToMatch)).Returns(456);

        Assert.That(_something.Anything(classToMatch), Is.EqualTo(456));
        Assert.That(_something.Anything(new SomeClass()), Is.EqualTo(0));
    }

    [Test]
    public void Return_result_with_only_one_matcher_for_that_type()
    {
        _something.Funky(Arg.Any<float>(), 12, "Lots", null).Returns(42);

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

        _something.Received().Echo(Arg.Any<int>());
    }

    [Test]
    public void Received_for_specific_argument()
    {
        _something.Echo(3);

        _something.Received().Echo(Arg.Is(3));
    }

    [Test]
    public void Received_for_argument_matching_predicate()
    {
        _something.Echo(7);

        _something.Received().Echo(Arg.Is<int>(x => x > 3));
    }

    [Test]
    public void Received_for_only_one_matcher_for_that_type()
    {
        _something.Funky(123.456f, 12, "Lots", null);

        _something.Received().Funky(Arg.Any<float>(), 12, "Lots", null);
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
        _something.Add(10, Arg.Any<int>()).Returns(1);

        Assert.That(_something.Add(10, 5), Is.EqualTo(1));
    }

    [Test]
    public void Received_should_compare_elements_for_params_arguments()
    {
        const string first = "first";
        const string second = "second";
        _something.WithParams(1, first, second);

        _something.Received().WithParams(1, first, second);
        _something.Received().WithParams(1, Arg.Any<string>(), second);
        _something.Received().WithParams(1, first, Arg.Any<string>());
        _something.Received().WithParams(1, [first, second]);
        _something.Received().WithParams(1, Arg.Any<string[]>());
        _something.Received().WithParams(1, Arg.Is<string[]>(x => x.Length == 2));
        _something.DidNotReceive().WithParams(2, first, second);
        _something.DidNotReceive().WithParams(2, first, Arg.Any<string>());
        _something.DidNotReceive().WithParams(1, first, first);
        _something.DidNotReceive().WithParams(1, null);
        _something.DidNotReceive().WithParams(1, Arg.Is<string[]>(x => x.Length > 3));
    }

    [Test]
    public void Should_allow_to_specify_any_for_ref_argument()
    {
        _something.MethodWithRefParameter(Arg.Any<int>(), ref Arg.Any<int>()).Returns(42);

        var refArg = 10;
        var result = _something.MethodWithRefParameter(0, ref refArg);
        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void Should_allow_to_specify_exact_is_for_ref_argument()
    {
        _something.MethodWithRefParameter(Arg.Any<int>(), ref Arg.Is(24)).Returns(42);

        var refArg = 24;
        var matchingResult = _something.MethodWithRefParameter(0, ref refArg);
        refArg = 10;
        var nonMatchingResult = _something.MethodWithRefParameter(0, ref refArg);
        Assert.That(matchingResult, Is.EqualTo(42));
        Assert.That(nonMatchingResult, Is.Not.EqualTo(42));
    }

    [Test]
    public void Should_allow_to_specify_is_expression_for_ref_argument()
    {
        _something.MethodWithRefParameter(Arg.Any<int>(), ref Arg.Is<int>(x => x == 24)).Returns(42);

        var refArg = 24;
        var matchingResult = _something.MethodWithRefParameter(0, ref refArg);
        refArg = 10;
        var nonMatchingResult = _something.MethodWithRefParameter(0, ref refArg);
        Assert.That(matchingResult, Is.EqualTo(42));
        Assert.That(nonMatchingResult, Is.Not.EqualTo(42));
    }

    [Test]
    public void Should_allow_to_specify_any_for_out_argument()
    {
        _something.MethodWithOutParameter(Arg.Any<int>(), out Arg.Any<int>()).Returns(42);

        var outArg = 10;
        var result1 = _something.MethodWithOutParameter(0, out outArg);
        var result2 = _something.MethodWithOutParameter(0, out int _);
        Assert.That(result1, Is.EqualTo(42));
        Assert.That(result2, Is.EqualTo(42));
    }

    [Test]
    public void Should_allow_to_specify_exact_is_for_out_argument()
    {
        _something.MethodWithOutParameter(Arg.Any<int>(), out Arg.Is(24)).Returns(42);

        var outArg = 24;
        var matchingResult = _something.MethodWithOutParameter(0, out outArg);
        var nonMatchingResult = _something.MethodWithOutParameter(0, out int _);
        Assert.That(matchingResult, Is.EqualTo(42));
        Assert.That(nonMatchingResult, Is.Not.EqualTo(42));
    }

    [Test]
    public void Should_allow_to_specify_is_expression_for_out_argument()
    {
        _something.MethodWithOutParameter(Arg.Any<int>(), out Arg.Is<int>(x => x == 24)).Returns(42);

        var outArg = 24;
        var matchingResult = _something.MethodWithOutParameter(0, out outArg);
        var nonMatchingResult = _something.MethodWithOutParameter(0, out int _);
        Assert.That(matchingResult, Is.EqualTo(42));
        Assert.That(nonMatchingResult, Is.Not.EqualTo(42));
    }

    [Test]
    public void Should_allow_to_check_received_using_properties_from_other_substitutes()
    {
        // Arrange
        var otherSubs = Substitute.For<ISomething>();
        otherSubs.SomeProperty.Returns(42);

        // Act
        _something.Echo(42);

        // Assert
        _something.Received().Echo(otherSubs.SomeProperty);
    }

    [Test]
    public void Throw_with_ambiguous_arguments_when_given_an_arg_matcher_and_a_default_arg_value_v1()
    {
        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            _something.Add(0, Arg.Any<int>()).Returns(1);
            Assert.Fail("Should not make it here, as it can't work out which arg the matcher refers to." +
                        "If it does this will throw an AssertionException rather than AmbiguousArgumentsException.");
        });
    }

    [Test]
    public void Throw_with_ambiguous_arguments_when_given_an_arg_matcher_and_a_default_arg_value_v2()
    {
        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            _something.MethodWithRefParameter(0, ref Arg.Any<int>()).Returns(42);
            Assert.Fail("Should not make it here, as it can't work out which arg the matcher refers to." +
                        "If it does this will throw an AssertionException rather than AmbiguousArgumentsException.");
        });
    }

    [Test]
    public void Throw_with_ambiguous_arguments_when_given_an_arg_matcher_and_a_default_arg_value_v3()
    {
        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            int defValue = 0;
            _something.MethodWithMultipleRefParameters(42, ref defValue, ref Arg.Any<int>()).Returns(42);
            Assert.Fail("Should not make it here, as it can't work out which arg the matcher refers to." +
                        "If it does this will throw an AssertionException rather than AmbiguousArgumentsException.");
        });
    }

    [Test]
    public void Should_add_list_of_all_pending_specifications_to_ambiguous_exception_message()
    {
        var exception = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            _something.Add(0, Arg.Is(42)).Returns(1);
        });

        Assert.That(exception.Message, Contains.Substring("42"));
    }

    [Test]
    public void Returns_should_work_with_params()
    {
        _something.WithParams(Arg.Any<int>(), Arg.Is<string>(x => x == "one")).Returns("fred");

        Assert.That(_something.WithParams(1, "one"), Is.EqualTo("fred"));
    }

    [Test]
    public void Resolve_setter_arg_matcher_with_more_specific_type_than_member_signature()
    {
        const string value = "some string";
        const string key = "key";

        _something[key] = value;

        _something.Received()[key] = Arg.Is(value);
    }

    [Test]
    public void Resolve_argument_matcher_for_more_specific_type()
    {
        _something.Anything("Hello");
        _something.Received().Anything(Arg.Any<string>());
        _something.DidNotReceive().Anything(Arg.Any<int>());
    }

    [Test]
    public void Set_returns_using_more_specific_type_matcher()
    {
        _something.Anything(Arg.Is<string>(x => x.Contains("world"))).Returns(123);

        Assert.That(_something.Anything("Hello world!"), Is.EqualTo(123));
        Assert.That(_something.Anything("Howdy"), Is.EqualTo(0));
        Assert.That(_something.Anything(2), Is.EqualTo(0));
    }

    [Test]
    public void Override_subclass_arg_matcher_when_returning_for_any_args()
    {
        _something.Anything(Arg.Any<string>()).ReturnsForAnyArgs(123);

        Assert.That(_something.Anything(new object()), Is.EqualTo(123));
    }

    [Test]
    public void Nullable_args_null_value()
    {
        _something.WithNullableArg(Arg.Any<int?>()).ReturnsForAnyArgs(123);

        Assert.That(_something.WithNullableArg(null), Is.EqualTo(123));
    }

    [Test]
    public void Nullable_args_notnull_value()
    {
        _something.WithNullableArg(Arg.Any<int?>()).ReturnsForAnyArgs(123);

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
            target.GetValue(0, Arg.Any<int>()).Returns(42);
        });
    }

    [Test]
    public void Should_fail_with_ambiguous_exception_if_params_boundary_is_crossed_scenario_2()
    {
        var target = Substitute.For<IMethodsWithParamsArgs>();

        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            target.GetValue(Arg.Any<int>(), 0).Returns(42);
        });
    }

    [Test]
    public void Should_correctly_use_matchers_crossing_the_params_boundary()
    {
        var target = Substitute.For<IMethodsWithParamsArgs>();
        target.GetValue(Arg.Is(0), Arg.Any<int>()).Returns(42);

        var result = target.GetValue(0, 100);

        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_more_specifications_than_arguments_scenario_1()
    {
        // This spec will be ignored, however it's good to let user know that test might not work how he expects.
        Arg.Is(42);

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            _something.Echo(10);
        });
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_more_specifications_than_arguments_scenario_2()
    {
        // This one will be used instead of Arg.Any<>(), causing the confusion.
        Arg.Is(42);

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            _something.Echo(Arg.Any<int>());
        });
    }

    [Test]
    public void Should_fail_with_redundant_exception_if_more_specifications_than_arguments_scenario_3()
    {
        // This spec will be ignored, however it's good to let user know that test might not work how he expects.
        Arg.Is(42);

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
        Arg.Is(42);

        var ex = Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            _something.Echo(_something.SomeProperty);
        });
        Assert.That(ex.Message, Contains.Substring("42"));
    }

    [Test]
    public void Redundant_argument_matcher_exception_should_contain_list_of_all_matchers()
    {
        Arg.Is(42);

        var ex = Assert.Throws<RedundantArgumentMatcherException>(() => { _something.Echo(Arg.Is(24)); });
        Assert.That(ex.Message, Contains.Substring("42"));
        Assert.That(ex.Message, Contains.Substring("24"));
    }

    [Test]
    public void Should_fail_with_redundant_exceptions_if_arg_matchers_misused()
    {
        var foo = Substitute.For<ISomething>();

        var misused = Arg.Is("test");

        Assert.Throws<RedundantArgumentMatcherException>(() =>
        {
            foo.Echo(2).Returns("42");
        });
    }

    public interface IInterfaceForAmbiguous
    {
        int MultipleMixedArgs(int arg1, double arg2, int arg3, double arg4);
        int ParamsArgs(int arg1, int arg2, params int[] rest);
        int GenericMethod<T>(T arg1, T arg2);
        int MethodWithRefAndOut(int arg1, int arg2, ref int refArg, out bool outArg);
        int this[int key1, int key2] { get; }
    }

    [Test]
    public void Should_show_already_resolved_matchers_in_ambiguous_exception_v1()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.MultipleMixedArgs(Arg.Any<int>(), Arg.Any<double>(), 0, 0d).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("MultipleMixedArgs(any Int32, any Double, ???, ???"));
    }

    [Test]
    public void Should_show_already_resolved_matchers_in_ambiguous_exception_v2()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.MultipleMixedArgs(42, Arg.Any<double>(), 123, 0d).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("MultipleMixedArgs(42, any Double, 123, ???)"));
    }

    [Test]
    public void Should_show_question_marks_for_non_resolved_matchers_in_ambiguous_exception()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.MultipleMixedArgs(Arg.Any<int>(), Arg.Any<double>(), 0, 0).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("MultipleMixedArgs(any Int32, any Double, ???, ???)"));
    }

    [Test]
    public void Should_show_question_mark_for_each_params_arg_in_ambiguous_exception_v1()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.ParamsArgs(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), 0, 0).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("ParamsArgs(any Int32, any Int32, any Int32, any Int32, ???, ???)"));
    }

    [Test]
    public void Should_show_question_mark_for_each_params_arg_in_ambiguous_exception_v2()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.ParamsArgs(Arg.Any<int>(), Arg.Any<int>(), 0).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("ParamsArgs(any Int32, any Int32, ???)"));
    }

    [Test]
    public void Should_show_generic_types_for_question_mark_line_in_ambiguous_exception()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.GenericMethod(null, Arg.Any<IEnumerable<List<int>>>()).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("GenericMethod<IEnumerable<List<Int32>>>(any IEnumerable<List<Int32>>, ???)"));
    }

    [Test]
    public void Should_show_method_signature_in_ambiguous_exception_v1()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.MultipleMixedArgs(0, 0d, Arg.Any<int>(), Arg.Any<double>()).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("MultipleMixedArgs(Int32, Double, Int32, Double)"));
    }

    [Test]
    public void Should_show_method_signature_in_ambiguous_exception_v2()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            int refValue = 42;
            foo.MethodWithRefAndOut(0, Arg.Any<int>(), ref refValue, out _).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("MethodWithRefAndOut(Int32, Int32, ref Int32, out Boolean)"));
    }

    [Test]
    public void Should_show_method_signature_in_ambiguous_exception_v3()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.ParamsArgs(0, Arg.Any<int>()).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("ParamsArgs(Int32, Int32, params Int32[])"));
    }

    [Test]
    public void Should_show_generic_method_signature_in_ambiguous_exception()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.GenericMethod(null, Arg.Any<List<int>>()).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("GenericMethod<List<Int32>>(List<Int32>, List<Int32>)"));
    }

    [Test]
    public void Should_show_actual_method_arguments_in_ambiguous_exception_v1()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.MultipleMixedArgs(0, 0d, Arg.Any<int>(), Arg.Any<double>()).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("MultipleMixedArgs(*0*, *0*, *0*, *0*)"));
    }

    [Test]
    public void Should_show_actual_method_arguments_in_ambiguous_exception_v2()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.ParamsArgs(42, 0, Arg.Any<int>(), 15).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("ParamsArgs(42, *0*, *0*, 15)"));
    }

    [Test]
    public void Should_show_actual_generic_method_arguments_in_ambiguous_exception()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo.GenericMethod(null, Arg.Any<List<int>>()).Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("GenericMethod<List<Int32>>(*<null>*, *<null>*)"));
    }

    [Test]
    public void Show_correctly_format_indexer_call_in_ambiguous_exception()
    {
        var foo = Substitute.For<IInterfaceForAmbiguous>();

        var ex = Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            foo[0, Arg.Any<int>()].Returns(42);
        });
        Assert.That(ex.Message, Does.Contain("[any Int32, ???]"));
    }

    public delegate string DelegateWithOutParameter(string input, out string result);

    [Test]
    public void Should_recognize_out_parameters_for_delegate_and_match_specification()
    {
        var subs = Substitute.For<DelegateWithOutParameter>();

        subs.Invoke(Arg.Any<string>(), out string _).Returns("42");

        var result = subs("foo", out string _);
        Assert.That(result, Is.EqualTo("42"));
    }

    public class IsFortyTwo : IArgumentMatcher<int>, IDescribeNonMatches
    {
        public static int Arg() => ArgumentMatcher.Enqueue(new IsFortyTwo());

        public bool IsSatisfiedBy(int argument) => argument == 42;

        public string DescribeFor(object argument) => $"{argument} is not forty two";
    }

    [Test]
    public void Supports_custom_argument_matchers()
    {
        var subs = Substitute.For<ISomething>();

        subs.Echo(IsFortyTwo.Arg()).Returns("42");

        var result = subs.Echo(42);
        Assert.That(result, Is.EqualTo("42"));
    }

    [Test]
    public void Supports_custom_argument_matcher_descriptions()
    {
        var subs = Substitute.For<ISomething>();

        subs.Echo(24);

        var ex = Assert.Throws<ReceivedCallsException>(() =>
        {
            //
            subs.Received().Echo(IsFortyTwo.Arg());
        });
        Assert.That(ex.Message, Contains.Substring("24 is not forty two"));
    }

    [Test]
    public void Supports_matching_generic_interface_bound_type_string_with_class_argument()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MyStringArgument();

        service.MyMethod(argument);

        service.Received().MyMethod(Arg.Any<IMyArgument<string>>());
    }

    [Test]
    public void Supports_matching_generic_interface_bound_type_custom_class_with_class_argument()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MySampleClassArgument();

        service.MyMethod(argument);

        service.Received().MyMethod(Arg.Any<IMyArgument<SampleClass>>());
    }

    [Test]
    public void Supports_matching_generic_interface_bound_type_custom_class_with_derived_class_argument()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MySampleDerivedClassArgument();

        service.MyMethod(argument);

        service.Received().MyMethod(Arg.Any<IMyArgument<SampleClass>>());
    }

    [Test]
    public void Supports_matching_custom_class_with_derived_class_argument()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MySampleDerivedClassArgument();

        service.MyMethod(argument);

        service.Received().MyMethod(Arg.Any<MySampleClassArgument>());
    }

    [Test]
    public void Supports_matching_generic_interface_bound_type_ArgAnyType_with_class_argument()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MyStringArgument();

        service.MyMethod(argument);

        service.Received().MyMethod(Arg.Any<IMyArgument<Arg.AnyType>>());
    }

    [Test]
    public void Supports_matching_generic_interface_bound_type_ArgAnyType_with_derived_class_argument()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MySampleDerivedClassArgument();

        service.MyMethod(argument);

        service.Received().MyMethod(Arg.Any<IMyArgument<Arg.AnyType>>());
    }

    [Test]
    public void Does_not_support_matching_ArgAny_of_type_derived_from_base_type_with_string_type_param_to_other_type_derived_from_base_type()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MyOtherStringArgument();

        service.MyMethod(argument);

        service.DidNotReceive().MyMethod(Arg.Any<MyStringArgument>());
    }

    [Test]
    public void Does_not_support_matching_ArgAny_of_type_derived_from_base_type_with_custom_type_param_to_other_type_derived_from_base_type()
    {
        var service = Substitute.For<IMyService>();
        var argument = new MyOtherSampleClassArgument();

        service.MyMethod(argument);

        service.DidNotReceive().MyMethod(Arg.Any<MySampleClassArgument>());
    }

    [SetUp]
    public void SetUp()
    {
        _something = Substitute.For<ISomething>();
    }

    public interface IMyService
    {
        void MyMethod<T>(IMyArgument<T> argument);
    }
    public interface IMyArgument<T> { }
    public class SampleClass { }
    public class MyStringArgument : IMyArgument<string> { }
    public class MyOtherStringArgument : IMyArgument<string> { }
    public class MySampleClassArgument : IMyArgument<SampleClass> { }
    public class MyOtherSampleClassArgument : IMyArgument<SampleClass> { }
    public class MySampleDerivedClassArgument : MySampleClassArgument { }
}