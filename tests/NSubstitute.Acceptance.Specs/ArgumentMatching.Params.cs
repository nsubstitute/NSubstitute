using System.Collections;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public partial class ArgumentMatching
{
    public interface IMethodsWithParams
    {
        int GetValue(int primary, params int[] others);
        string WithParams(int i, params string[] labels);
        void WithArrayParams(params object[] args);
        void WithEnumerableParams(params IEnumerable<object> values);
        void WithListParams(params List<object> values);
        void WithArrayListParams(params ArrayList values);
        void WithCustomCollectionParams(params CustomParamsCollection values);
    }

    // Implements only the non-generic IEnumerable (no IEnumerable<T>) - still a valid params
    // collection type per the language, since the compiler falls back to the single-argument
    // Add overload to determine the element type.
    public class CustomParamsCollection : IEnumerable
    {
        private readonly ArrayList _items = new();
        public void Add(object item) => _items.Add(item);
        public IEnumerator GetEnumerator() => _items.GetEnumerator();
    }

    [Test]
    public void Received_should_compare_elements_for_params_arguments()
    {
        var target = Substitute.For<IMethodsWithParams>();
        const string first = "first";
        const string second = "second";
        target.WithParams(1, first, second);

        target.Received().WithParams(1, first, second);
        target.Received().WithParams(1, Arg.Any<string>(), second);
        target.Received().WithParams(1, first, Arg.Any<string>());
        target.Received().WithParams(1, [first, second]);
        target.Received().WithParams(1, Arg.Any<string[]>());
        target.Received().WithParams(1, Arg.Is<string[]>(x => x.Length == 2));
        target.DidNotReceive().WithParams(2, first, second);
        target.DidNotReceive().WithParams(2, first, Arg.Any<string>());
        target.DidNotReceive().WithParams(1, first, first);
        target.DidNotReceive().WithParams(1, null);
        target.DidNotReceive().WithParams(1, Arg.Is<string[]>(x => x.Length > 3));
    }

    [Test]
    public void Returns_should_work_with_params()
    {
        var target = Substitute.For<IMethodsWithParams>();
        target.WithParams(Arg.Any<int>(), Arg.Is<string>(x => x == "one")).Returns("fred");

        Assert.That(target.WithParams(1, "one"), Is.EqualTo("fred"));
    }

    [Test]
    public void Should_fail_with_ambiguous_exception_if_params_boundary_is_crossed_scenario_1()
    {
        var target = Substitute.For<IMethodsWithParams>();

        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            target.GetValue(0, Arg.Any<int>()).Returns(42);
        });
    }

    [Test]
    public void Should_fail_with_ambiguous_exception_if_params_boundary_is_crossed_scenario_2()
    {
        var target = Substitute.For<IMethodsWithParams>();

        Assert.Throws<AmbiguousArgumentsException>(() =>
        {
            target.GetValue(Arg.Any<int>(), 0).Returns(42);
        });
    }

    [Test]
    public void Should_correctly_use_matchers_crossing_the_params_boundary()
    {
        var target = Substitute.For<IMethodsWithParams>();
        target.GetValue(Arg.Is(0), Arg.Any<int>()).Returns(42);

        var result = target.GetValue(0, 100);

        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void Should_match_array_params_the_same_way_as_a_baseline()
    {
        var target = Substitute.For<IMethodsWithParams>();
        var arg = new object();

        target.WithArrayParams(arg);

        target.Received().WithArrayParams(arg);
    }

    [Test]
    public void Should_match_received_call_with_single_element_params_collection()
    {
        var target = Substitute.For<IMethodsWithParams>();
        var arg = new object();

        target.WithEnumerableParams(arg);

        target.Received().WithEnumerableParams(arg);
    }

    [Test]
    public void Should_match_received_call_with_multi_element_params_collection()
    {
        var target = Substitute.For<IMethodsWithParams>();
        var arg1 = new object();
        var arg2 = new object();

        target.WithEnumerableParams(arg1, arg2);

        target.Received().WithEnumerableParams(arg1, arg2);
    }

    [Test]
    public void Should_not_match_received_call_with_different_params_collection_contents()
    {
        var target = Substitute.For<IMethodsWithParams>();

        target.WithEnumerableParams(new object());

        Assert.Throws<ReceivedCallsException>(() => target.Received().WithEnumerableParams(new object()));
    }

    [Test]
    public void Should_match_received_call_with_params_collection_declared_as_concrete_list_type()
    {
        var target = Substitute.For<IMethodsWithParams>();
        var arg = new object();

        target.WithListParams(arg);

        target.Received().WithListParams(arg);
    }

    [Test]
    public void Should_match_received_call_with_params_collection_declared_as_non_generic_bcl_collection()
    {
        var target = Substitute.For<IMethodsWithParams>();
        var arg = new object();

        target.WithArrayListParams(arg);

        target.Received().WithArrayListParams(arg);
    }

    [Test]
    public void Should_match_received_call_with_params_collection_declared_as_custom_non_generic_collection()
    {
        var target = Substitute.For<IMethodsWithParams>();
        var arg = new object();

        target.WithCustomCollectionParams(arg);

        target.Received().WithCustomCollectionParams(arg);
    }
}
