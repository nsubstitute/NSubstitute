using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class AutoValuesForSubs
{
    private ISample _sample;

    public class PureVirtualClass { public virtual void Foo() { } }
    public class NonVirtualClass { public void Bar() { } }
    public sealed class SealedClass { }
    delegate ISample SampleFactory();

    public interface ISample
    {
        int[] GetNumbers();
        IEnumerable<object> GetObjects();
        string Name { get; set; }
        List<string> ListOfStrings { get; set; }
        int? GetNullableNumber();
        PureVirtualClass VirtualClass { get; set; }
        NonVirtualClass NonVirtualClass { get; set; }
        SealedClass SealedClass { get; set; }
        IQueryable<int> Queryable();
    }

    [SetUp]
    public void SetUp()
    {
        _sample = Substitute.For<ISample>();
    }

    [Test]
    public void Should_auto_return_empty_array()
    {
        Assert.That(_sample.GetNumbers().Length, Is.EqualTo(0));
    }

    [Test]
    public void Should_auto_return_empty_enumerable()
    {
        Assert.That(_sample.GetObjects(), Is.Not.Null);
        Assert.That(_sample.GetObjects().Count(), Is.EqualTo(0));
    }

    [Test]
    public void Should_auto_return_empty_string()
    {
        Assert.That(_sample.Name.Length, Is.EqualTo(0));
    }

    [Test]
    public void Should_return_null_for_nullables()
    {
        Assert.That(_sample.GetNullableNumber(), Is.Null);
    }

    [Test]
    public void Should_return_same_empty_value_for_auto_values_for_reference_types()
    {
        var autoArrayValue = _sample.GetNumbers();
        Assert.That(_sample.GetNumbers(), Is.SameAs(autoArrayValue));
    }

    [Test]
    public void Should_return_substitute_for_pure_virtual_class()
    {
        Assert.That(_sample.VirtualClass, Is.Not.Null);
    }

    [Test]
    public void Should_return_default_value_for_non_virtual_class()
    {
        Assert.That(_sample.NonVirtualClass, Is.Null);
    }

    [Test]
    public void Should_return_default_value_for_sealed_class()
    {
        Assert.That(_sample.SealedClass, Is.Null);
    }

    [Test]
    [Pending, Explicit]
    public void Should_auto_return_empty_string_list()
    {
        Assert.That(_sample.ListOfStrings, Is.Not.Null);
        Assert.That(_sample.ListOfStrings.Count(), Is.EqualTo(0));
    }

    [Test]
    public void Should_auto_return_a_substitute_from_a_function_that_returns_an_interface()
    {
        var x = Substitute.For<Func<ISample>>();
        var returnedFromFunc = x();
        Assert.That(returnedFromFunc, Is.Not.Null);
        AssertObjectIsASubstitute(returnedFromFunc);
    }

    [Test]
    public void Should_auto_return_an_empty_string_from_a_func_that_returns_a_string()
    {
        var x = Substitute.For<Func<ISample, string>>();

        Assert.That(x(_sample).Length, Is.EqualTo(0));
    }

    [Test]
    public void Should_auto_return_for_iqueryable()
    {
        var sample = Substitute.For<ISample>();
        Assert.That(sample.Queryable().Select(x => x + 1).ToList(), Is.Empty);
        Assert.That(sample.Queryable().Expression, Is.Not.Null);
    }

    [Test]
    public void Should_auto_return_a_substitute_from_a_func_that_returns_a_pure_virtual_class()
    {
        var x = Substitute.For<Func<PureVirtualClass>>();
        var returnedFromFunc = x();

        Assert.That(returnedFromFunc, Is.Not.Null);
        AssertObjectIsASubstitute(returnedFromFunc);
    }

    [Test]
    public void Should_not_auto_return_a_substitute_from_a_func_that_returns_a_non_virtual_class()
    {
        var x = Substitute.For<Func<NonVirtualClass>>();
        var returnedFromFunc = x();

        Assert.That(returnedFromFunc, Is.Null);
    }

    [Test]
    public void Should_auto_return_a_substitute_from_a_delegate_that_returns_an_interface()
    {
        var x = Substitute.For<SampleFactory>();

        var returnedFromFunc = x();

        Assert.That(returnedFromFunc, Is.Not.Null);
        AssertObjectIsASubstitute(returnedFromFunc);
    }

    [Test]
    public void Should_auto_return_a_value_from_a_task()
    {
        var sub = Substitute.For<IFooWithTasks>();
        var task = sub.GetIntAsync();
        Assert.That(task.IsCompleted, Is.True);
        Assert.That(task.Result, Is.EqualTo(0));
    }

    [Test]
    public void Should_auto_return_an_autosub_from_a_task()
    {
        var sub = Substitute.For<IFooWithTasks>();

        var task = sub.GetSample();

        Assert.That(task.IsCompleted, Is.True);
        var sample = task.Result;
        AssertObjectIsASubstitute(sample);
        sample.Name = "test";
        Assert.That(sample.Name, Is.EqualTo("test"));
    }

    [Test]
    public void Should_auto_return_a_completed_non_generic_task()
    {
        var sub = Substitute.For<IFooWithTasks>();
        var task = sub.GetNonGenericTask();
        Assert.That(task.IsCompleted, Is.True);
    }

    public interface IFooWithTasks
    {
        Task<ISample> GetSample();
        Task<int> GetIntAsync();
        Task GetNonGenericTask();
    }

    [Test]
    public void Should_auto_return_an_observable()
    {
        var sub = Substitute.For<IFooWithObservable>();
        int sample = -42;
        sub.GetInts().Subscribe(new AnonymousObserver<int>(x => sample = x));
        Assert.That(sample, Is.EqualTo(0));
    }

    [Test]
    public void Should_auto_return_an_autosub_from_an_observable()
    {
        var sub = Substitute.For<IFooWithObservable>();
        ISample sample = null;
        sub.GetSamples().Subscribe(new AnonymousObserver<ISample>(x => sample = x));
        AssertObjectIsASubstitute(sample);

        sample.Name = "test";
        Assert.That(sample.Name, Is.EqualTo("test"));
    }

    [Test]
    public void Multiple_calls_to_observable_method()
    {
        var sub = Substitute.For<IFooWithObservable>();
        ISample sample1 = null;
        ISample sample2 = null;
        sub.GetSamples().Subscribe(new AnonymousObserver<ISample>(x => sample1 = x));
        sub.GetSamples().Subscribe(new AnonymousObserver<ISample>(x => sample2 = x));
        AssertObjectIsASubstitute(sample1);
        AssertObjectIsASubstitute(sample2);

        Assert.That(sample1, Is.SameAs(sample2));
    }

    public interface IFooWithObservable
    {
        IObservable<int> GetInts();
        IObservable<ISample> GetSamples();
    }

    //Copied from NSubstitute.Specs.AnonymousObserver (PR #137)
    private class AnonymousObserver<T>(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null) : IObserver<T>
    {
        Action<T> _onNext = onNext ?? (_ => { });
        Action<Exception> _onError = onError ?? (_ => { });
        Action _onCompleted = onCompleted ?? (() => { });

        public void OnNext(T value) { _onNext(value); }
        public void OnError(Exception error) { _onError(error); }
        public void OnCompleted() { _onCompleted(); }
    }

    private static void AssertObjectIsASubstitute<T>(T obj) where T : class
    {
        Assert.That(obj.ReceivedCalls(), Is.Empty);
    }
}